#include <asyncnetworkengine/asyncnetworkengine.hpp>
#include <curl/curl.h>
#include <iostream>

//static variables
std::mutex globalMutex;

size_t curlWriteMemory(void *contents, size_t size, size_t nmemb, void *userp)
{
    auto wSize = size * nmemb;
    auto* memory = static_cast<std::vector<char>*>(userp);
    auto cSize = memory->size();
    auto mSize = cSize + wSize;
    memory->resize(mSize);
    auto p = &(memory->data()[cSize]);
    memcpy(p,contents,mSize);
    return wSize;
}

void AsyncNetworkEngine::Download(std::string url, std::function<void(bool,std::vector<char>)> callback)
{
    std::vector<std::string> files = { url };
    Download(files, [callback](bool result,std::vector<std::vector<char>> files)
    {
        if(!result)
        {
            std::vector<char> f;
            callback(result, f);
            return;
        }
        callback(result, files[0]);
    });
}

void AsyncNetworkEngine::Download(std::vector<std::string> urls, std::function<void(bool,std::vector<std::vector<char>>)> callback)
{
    globalMutex.lock();

    std::vector<std::vector<char>> files;

    if(curl_global_init(CURL_GLOBAL_DEFAULT) != CURLE_OK)
    {
        std::cout << "curl_global_init" << std::endl;
        curl_global_cleanup();
        globalMutex.unlock();
        callback(false, files);
        return;
    }

    for(int i=0;i<urls.size();i++)
    {
        std::vector<char> memory;

        auto curl = curl_easy_init();
        if(!curl)
        {
            std::cout << "curl_easy_init" << std::endl;
            curl_easy_cleanup(curl);
            curl_global_cleanup();
            globalMutex.unlock();
            callback(false, files);
            return;
        }

        curl_easy_setopt(curl, CURLOPT_URL, "https://ase-test-bucket.s3.amazonaws.com/file.txt\0");
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, curlWriteMemory);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &memory);

        auto result = curl_easy_perform(curl);
        if(result != CURLE_OK)
        {
            std::cout << "curl_easy_perform::" << result << std::endl;
            curl_easy_cleanup(curl);
            curl_global_cleanup();
            globalMutex.unlock();
            callback(false, files);
            return;
        }
        files.push_back(memory);
        curl_easy_cleanup(curl);
    }

    curl_global_cleanup();
    globalMutex.unlock();
    callback(true, files);
}