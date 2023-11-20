#pragma once

#include "CoreMinimal.h"
#include "Http.h"

template<typename TRqt, typename TResp>
class AsyncNetworkEngine
{
public:
    void Send(TRqt Request);

private:
};

// Implementação da função Send
template<typename TRqt, typename TResp>
void AsyncNetworkEngine<TRqt, TResp>::Send(TRqt Request)
{
    TSharedRef<IHttpRequest> HttpRequest = FHttpModule::Get().CreateRequest();

    TArray<uint8> buffer;
    buffer.SetNumUninitialized(Request.ByteSizeLong(), false);
    Request.SerializeToArray(buffer.GetData(), Request.ByteSizeLong());
    HttpRequest->SetURL(TEXT("https://phy-dev-api.phyengine.com/phy-dev-gettest"));

    HttpRequest->SetVerb(TEXT("POST"));
    //HttpRequest->SetContent(buffer.GetData(), buffer.Num());
    //HttpRequest->SetContent(buffer.GetData(), buffer.Num());
    HttpRequest->SetContent(buffer);

    // Adicione a vinculação de callback se necessário
    // HttpRequest->OnProcessRequestComplete().BindUObject(this, &AsyncNetworkEngine::OnRequestComplete);

    HttpRequest->ProcessRequest();
}
