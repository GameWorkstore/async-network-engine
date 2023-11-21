#include "AsyncNetworkEngine.h"
#include "CoreMinimal.h"
#include "Http.h"
#include "../../../../../Source/UnrealSDK/Proto/asyncrpc.pb.h"

template<typename TRqt, typename TResp>
void AsyncNetworkEngine<TRqt, TResp>::Send(FString url, TRqt Request, CallbackFunction callback)
{
    TSharedRef<IHttpRequest> HttpRequest = FHttpModule::Get().CreateRequest();

    TArray<uint8> buffer;

    buffer.SetNumUninitialized(Request.ByteSizeLong(), false);

    Request.SerializeToArray(buffer.GetData(), Request.ByteSizeLong());

    HttpRequest->SetURL(url);

    HttpRequest->SetVerb(TEXT("POST"));

    HttpRequest->SetContent(buffer);

    HttpRequest->OnProcessRequestComplete().BindUObject(this, callback);

    HttpRequest->ProcessRequest();
}


