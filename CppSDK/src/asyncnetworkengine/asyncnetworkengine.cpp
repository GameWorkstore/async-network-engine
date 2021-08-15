#include <asyncnetworkengine/asyncnetworkengine.hpp>
#include <curl/curl.h>

#define byte char;
//static variables
mutex globalMutex;

size_t curlWriteMemory(void *contents, size_t size, size_t nmemb, void *userp)
{
    auto wSize = size * nmemb;
    auto* memory = static_cast<vector<char>*>(userp);
    auto cSize = memory->size();
    auto mSize = cSize + wSize;
    memory->resize(mSize);
    auto p = &(memory->data()[cSize]);
    memcpy(p,contents,mSize);
    return wSize;
}

void AsyncNetworkEngine::Download(string url, function<void(bool,vector<char>)> callback)
{
    vector<string> files = { url };
    Download(files, [callback](bool result,vector<vector<char>> files)
    {
        if(!result)
        {
            vector<char> f;
            callback(result, f);
            return;
        }
        callback(result, files[0]);
    });
}

void AsyncNetworkEngine::Download(vector<string> urls, function<void(bool,vector<vector<char>>)> callback)
{
    globalMutex.lock();

    vector<vector<char>> files;

    if(curl_global_init(CURL_GLOBAL_ALL) != CURLE_OK)
    {
        curl_global_cleanup();
        globalMutex.unlock();
        callback(false, files);
        return;
    }

    for(int i=0;i<urls.size();i++)
    {
        vector<char> memory;

        auto curl = curl_easy_init();
        if(!curl)
        {
            curl_easy_cleanup(curl);
            curl_global_cleanup();
            globalMutex.unlock();
            callback(false, files);
            return;
        }

        curl_easy_setopt(curl, CURLOPT_URL, urls[i].c_str());
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, curlWriteMemory);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &memory);

        if(curl_easy_perform(curl) != CURLE_OK)
        {
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