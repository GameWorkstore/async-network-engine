#pragma once
#include <string>
#include <functional>
#include <vector>
#include <mutex>
#include <type_traits>
#include <google/protobuf/message_lite.h>
#if defined(__UNREAL__)
#include "CoreMinimal.h"
#include "Http.h"
#else
#include <curl/curl.h>
#endif
#include "base64.h"
#include "asyncrpc.pb.h"

#define CURL_LONG_ON 1L
#define CURL_LONG_OFF 0L

namespace GameWorkstore
{
	namespace AsyncNetworkEngine
	{
		typedef std::function<void(bool, std::vector<char>)> DownloadSingleCallback;
		typedef std::function<void(bool, std::vector<std::vector<char>>)> DownloadMultipleCallback;

		class AsyncNetworkStatic
		{
		public:
			static bool Init();
			static size_t CurlWriteMemory(void* contents, size_t size, size_t nmemb, void* userp);
			static void Download(std::string url, DownloadSingleCallback callback);
			static void Download(std::vector<std::string> urls, DownloadMultipleCallback callback);
		};

		template<class T, typename U>
		class AsyncNetworkEngine
		{
			static_assert(std::is_base_of<google::protobuf::MessageLite, T>::value, "T must derive from google::protobuf::MessageLite");
			static_assert(std::is_base_of<google::protobuf::MessageLite, U>::value, "U must derive from google::protobuf::MessageLite");
		public:
#if defined(__UNREAL__)
			DECLARE_DELEGATE_ThreeParams(FAsyncNetworkCallback, Transmission, U, GenericErrorResponse)

			static void ProcessRequestComplete(FHttpRequestPtr httpRequest, FHttpResponsePtr httpResponse, bool bWasSuccessful, FAsyncNetworkCallback callback)
			{
				U resp;
				GenericErrorResponse error;
				if (!bWasSuccessful)
				{
					switch (httpResponse->GetResponseCode())
					{
					case (int)Transmission::ErrorInternalServer:
						error.set_error("ErrorInternalServer");
						callback.Execute(Transmission::ErrorInternalServer, resp, error);
						return;
					default:
						error.set_error("NotSpecified");
						callback.Execute(Transmission::NotSpecified, resp, error);
						return;
					}
				}
				auto decoded = base64_decode(httpResponse->GetContent());
				if (!resp.ParseFromArray(decoded.GetData(), (int)decoded.Num()))
				{
					error.set_error("ErrorParser");
					callback.Execute(Transmission::ErrorParser, resp, error);
					return;
				}

				callback.Execute(Transmission::Success, resp, error);
			}

			static void Send(FString url, T rqt, FAsyncNetworkCallback callback)
#else
			static void Send(std::string url, T rqt, std::function<void(Transmission, U, GenericErrorResponse)> callback)
#endif
			{
#if defined(__UNREAL__)
				TSharedRef<IHttpRequest> httpRequest = FHttpModule::Get().CreateRequest();

				TArray<uint8> buffer;
				buffer.SetNumUninitialized(rqt.ByteSizeLong(), false);

				rqt.SerializeToArray(buffer.GetData(), rqt.ByteSizeLong());

				httpRequest->SetURL(url);
				httpRequest->SetVerb(TEXT("POST"));
				httpRequest->SetContent(buffer);
				httpRequest->SetHeader(TEXT("Content-Type"), TEXT("application/octet-stream"));
				httpRequest->OnProcessRequestComplete().BindStatic(&ProcessRequestComplete, callback);
				httpRequest->ProcessRequest();
#else
				U resp;
				GenericErrorResponse error;
				if (!AsyncNetworkStatic::Init())
				{
					error.set_error("curl global init failure");
					callback(Transmission::ErrorProtocol, resp, error);
					return;
				}

				auto curl = curl_easy_init();
				if (!curl)
				{
					curl_easy_cleanup(curl);
					error.set_error("curl init failure");
					callback(Transmission::ErrorProtocol, resp, error);
					return;
				}

				auto rqtb = reinterpret_cast<google::protobuf::MessageLite*>(&rqt);
				auto size = (long)rqtb->ByteSizeLong();
				std::unique_ptr<char[]> serialized(new char[size]);
				rqtb->SerializeToArray(serialized.get(), size);

				std::vector<char> receiverBuffer;

				curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
				curl_easy_setopt(curl, CURLOPT_NOPROGRESS, CURL_LONG_ON);
				// receiver configuration
				curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, AsyncNetworkStatic::CurlWriteMemory);
				curl_easy_setopt(curl, CURLOPT_WRITEDATA, &receiverBuffer);
				// post configuration
				curl_easy_setopt(curl, CURLOPT_POST, CURL_LONG_ON);
				curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, size);
				curl_easy_setopt(curl, CURLOPT_COPYPOSTFIELDS, serialized.get());

				auto result = curl_easy_perform(curl);
				if (result != CURLE_OK)
				{
					curl_easy_cleanup(curl);
					error.set_error("curl perform failure with error:" + std::to_string(result));
					callback(Transmission::NotSpecified, resp, error);
					return;

				}
				long http_code = -1;
				curl_easy_getinfo(curl, CURLINFO_RESPONSE_CODE, &http_code);
				curl_easy_cleanup(curl);
				auto respb = reinterpret_cast<google::protobuf::MessageLite*>(&resp);
				switch (result)
				{
				case CURLE_OK:
				{
					if (receiverBuffer.size() <= 0)
					{
						callback(Transmission::ErrorNoData, resp, error);
						return;
					}
					std::vector<uint8_t> decoded = base64_decode(receiverBuffer);
					if (!respb->ParseFromArray(decoded.data(), (int)decoded.size()))
					{
						callback(Transmission::ErrorParser, resp, error);
						return;
					}
					callback(Transmission::Success, resp, error);
					return;
				}
				default:
					callback(Transmission::ErrorConnection, resp, error);
					break;
			}
#endif
		}
	};
}
}