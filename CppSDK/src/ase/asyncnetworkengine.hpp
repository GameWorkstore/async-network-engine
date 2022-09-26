#pragma once
#include <string>
#include <functional>
#include <vector>
#include <mutex>
#include <curl/curl.h>
#include "asyncrpc.pb.h"

namespace GameWorkstore
{
    namespace AsyncNetworkEngine
    {
        typedef std::function<void(bool,std::vector<char>)> DownloadSingleCallback;
        typedef std::function<void(bool,std::vector<std::vector<char>>)> DownloadMultipleCallback;
        
        class AsyncNetworkStatic
        {
        public:
            static bool Init();
            static size_t CurlWriteMemory(void *contents, size_t size, size_t nmemb, void *userp);
            static void Download(std::string url, DownloadSingleCallback callback);
            static void Download(std::vector<std::string> urls, DownloadMultipleCallback callback);
        };

        template<typename T, typename U>
        class AsyncNetworkEngine
        {
        public:
            static void Send(std::string url, T rqt, std::function<void(Transmission,U,GenericErrorResponse)> callback)
            {
                U resp;
                GenericErrorResponse error;
                if(!AsyncNetworkStatic::Init())
                {
                    error.set_error("curl global init failure");
                    callback(Transmission::ErrorProtocol,resp,error);
                    return;
                }

                auto curl = curl_easy_init();
                if(!curl)
                {
                    curl_easy_cleanup(curl);
                    error.set_error("curl init failure");
                    callback(Transmission::ErrorProtocol,resp,error);
                    return;
                }

                std::vector<char> memory;

                curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
                curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, AsyncNetworkStatic::CurlWriteMemory);
                curl_easy_setopt(curl, CURLOPT_WRITEDATA, &memory);

                auto result = curl_easy_perform(curl);
                curl_easy_cleanup(curl);
                switch (result)
                {
                case CURLE_OK:
                    callback(Transmission::Success, resp, error);
                    return;
                default:
                    error.set_error("curl perform failure with error:" + std::to_string(result));
                    callback(Transmission::NotSpecified, resp, error);
                    return;
                }
            }
        };
    }
}