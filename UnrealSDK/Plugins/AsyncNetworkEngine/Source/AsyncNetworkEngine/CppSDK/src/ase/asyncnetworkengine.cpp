#include "asyncnetworkengine.h"
//temporary for debug
#include <iostream>

using namespace GameWorkstore::AsyncNetworkEngine;

//static variables
std::mutex globalMutex;
bool isInitialized = false;

size_t AsyncNetworkStatic::CurlWriteMemory(void *contents, size_t size, size_t nmemb, void *userp)
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

bool AsyncNetworkStatic::Init()
{
    globalMutex.lock();
    if(isInitialized)
    {
        globalMutex.unlock();
        return isInitialized;
    }
#if defined(__UNREAL__)
#else
    isInitialized = curl_global_init(CURL_GLOBAL_DEFAULT) == CURLE_OK;
#endif
    globalMutex.unlock();
    return isInitialized;
}

void AsyncNetworkStatic::Download(std::string url, std::function<void(bool,std::vector<char>)> callback)
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

void AsyncNetworkStatic::Download(std::vector<std::string> urls, std::function<void(bool,std::vector<std::vector<char>>)> callback)
{
    std::vector<std::vector<char>> files;

    if(!Init())
    {
        std::cout << "curl_global_init" << std::endl;
        callback(false, files);
        return;
    }

    for(int i=0;i<urls.size();i++)
    {
        std::vector<char> memory;
#if defined(__UNREAL__)
#else
        auto curl = curl_easy_init();
        if(!curl)
        {
            std::cout << "curl_easy_init" << std::endl;
            curl_easy_cleanup(curl);
            callback(false, files);
            return;
        }

        curl_easy_setopt(curl, CURLOPT_URL, urls[0].c_str());
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, CurlWriteMemory);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &memory);

        auto result = curl_easy_perform(curl);
        if(result != CURLE_OK)
        {
            std::cout << "curl_easy_perform::" << result << std::endl;
            curl_easy_cleanup(curl);
            callback(false, files);
            return;
        }
        files.push_back(memory);
        curl_easy_cleanup(curl);
#endif
    }

    callback(true, files);
}