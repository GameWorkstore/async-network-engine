#pragma once

#include "../../../../../Source/UnrealSDK/Proto/asyncrpc.pb.h"

template<typename TRqt, typename TResp>
class AsyncNetworkEngine
{
public:
    
    typedef void (*CallbackFunction)(GameWorkstore::AsyncNetworkEngine::Transmission, TResp, GameWorkstore::AsyncNetworkEngine::GenericErrorResponse);

    static void Send(FString url, TRqt Request, CallbackFunction callback);

private:
};
