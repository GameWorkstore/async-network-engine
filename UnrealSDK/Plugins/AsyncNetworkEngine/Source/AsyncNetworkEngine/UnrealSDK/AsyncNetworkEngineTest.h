#pragma once

#include "../CppSDK/src/ase/asyncrpc.pb.h"
#include "Http.h"

class AsyncNetworkEngineTest
{
public:

    static void OnResponseReceived(GameWorkstore::AsyncNetworkEngine::GenericResponse Response)
    {
        UE_LOG(LogTemp, Warning, TEXT("Tamanho da resposta: %d"), Response.ByteSizeLong());

        //UE_LOG(LogTemp, Warning, TEXT("Minha string: %s"), Response.messege();
    }

private:
};
