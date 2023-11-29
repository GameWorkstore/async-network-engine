#pragma once

#include "../CppSDK/src/ase/asyncrpc.pb.h"
#include "Http.h"

template<typename TRqt, typename TResp>
class AsyncNetworkEngine
{
public:
    
    typedef void (*CallbackFunction)(TResp);

    static void Send(FString url, TRqt Request, CallbackFunction callback)
    {
        TSharedRef<IHttpRequest> HttpRequest = FHttpModule::Get().CreateRequest();

        TArray<uint8> buffer;

        buffer.SetNumUninitialized(Request.ByteSizeLong(), false);

        Request.SerializeToArray(buffer.GetData(), Request.ByteSizeLong());

        HttpRequest->SetURL(url);

        HttpRequest->SetVerb(TEXT("POST"));

        HttpRequest->SetContent(buffer);

        HttpRequest->SetHeader(TEXT("Content-Type"), TEXT("application/octet-stream"));

        HttpRequest->OnProcessRequestComplete().BindStatic(&AsyncNetworkEngine::ProcessRequestComplete, callback);

        HttpRequest->ProcessRequest();
    }

    static void ProcessRequestComplete(FHttpRequestPtr Request, FHttpResponsePtr Response, bool bWasSuccessful, CallbackFunction Callback)
    {
        TResp responseObj;

        bool parseSuccess = responseObj.ParseFromArray(Response->GetContent().GetData(), Response->GetContentLength());

        if (!parseSuccess) 
        {
            
        }

        UE_LOG(LogTemp, Warning, TEXT("Tamanho do dado: %d"), Response->GetContentLength());

        if (Callback)
        {
            Callback(responseObj);
        }
    }

private:
   // static_assert(std::is_base_of<::google::protobuf::Message, TRqt>::value,
};