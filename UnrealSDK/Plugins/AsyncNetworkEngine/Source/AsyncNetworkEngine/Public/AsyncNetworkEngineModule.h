#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

class FAsyncNetworkEngineModule : public IModuleInterface
{
public:
    /** IModuleInterface implementation */
    virtual void StartupModule() override;
    virtual void ShutdownModule() override;

    //template<typename TRqt, typename TResp>
   // void Send(const FString& Endpoint, const TRqt& Request);
};

