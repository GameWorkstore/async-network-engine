#pragma once

#include "CoreMinimal.h"
#include "Http.h"

template<typename TRqt, typename TResp>
class AsyncNetworkEngine
{
public:
    static void Send(FString url, TRqt Request, TFunction<void> callback);

private:
};

//"https://phy-dev-api.phyengine.com/phy-dev-gettest"

template<typename TRqt, typename TResp>
void AsyncNetworkEngine<TRqt, TResp>::Send(FString url, TRqt Request, TFunction<void> callback)
{
    TSharedRef<IHttpRequest> HttpRequest = FHttpModule::Get().CreateRequest();

    TArray<uint8> buffer;
    buffer.SetNumUninitialized(Request.ByteSizeLong(), false);
    Request.SerializeToArray(buffer.GetData(), Request.ByteSizeLong());
    HttpRequest->SetURL(TEXT());

    HttpRequest->SetVerb(TEXT("POST"));
    //HttpRequest->SetContent(buffer.GetData(), buffer.Num());
    //HttpRequest->SetContent(buffer.GetData(), buffer.Num());
    HttpRequest->SetContent(buffer);

    // Adicione a vincula��o de callback se necess�rio
    // HttpRequest->OnProcessRequestComplete().BindUObject(this, &AsyncNetworkEngine::OnRequestComplete);

    HttpRequest->ProcessRequest();
}
