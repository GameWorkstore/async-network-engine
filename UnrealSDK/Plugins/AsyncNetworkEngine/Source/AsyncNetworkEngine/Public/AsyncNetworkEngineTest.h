#pragma once

#include "../../../../../Source/UnrealSDK/Proto/asyncrpc.pb.h"
#include "../../../../Protobuf/Source/Protobuf/Public/Protobuf.h"
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
