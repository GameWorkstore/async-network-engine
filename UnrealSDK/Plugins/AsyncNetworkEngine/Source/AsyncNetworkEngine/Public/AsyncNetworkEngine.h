#pragma once

#include "../../../../../Source/UnrealSDK/Proto/asyncrpc.pb.h"
#include "../../../../Protobuf/Source/Protobuf/Public/Protobuf.h"

template<typename TRqt, typename TResp>
class AsyncNetworkEngine
{
public:
    
    typedef void (*CallbackFunction)(GameWorkstore::AsyncNetworkEngine::Transmission, TResp, GameWorkstore::AsyncNetworkEngine::GenericErrorResponse);

    static void Send(FString url, TRqt Request, CallbackFunction callback);

private:
   // static_assert(std::is_base_of<::google::protobuf::Message, TRqt>::value,
};
