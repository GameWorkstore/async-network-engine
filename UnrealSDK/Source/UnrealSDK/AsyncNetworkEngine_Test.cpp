// Fill out your copyright notice in the Description page of Project Settings.
#include "AsyncNetworkEngineGameMode.h"
#include "CoreMinimal.h"
#include "Misc/AutomationTest.h"
#include "Proto/asyncrpc.pb.h"
#include "../Plugins/AsyncNetworkEngine/Source/AsyncNetworkEngine/Public/AsyncNetworkEngine.h"

IMPLEMENT_SIMPLE_AUTOMATION_TEST(FASEGoogleProtobufTest, "ASE.GoogleProtobufTest", EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

bool FASEGoogleProtobufTest::RunTest(const FString& Parameters)
{
	// init
	GameWorkstore::AsyncNetworkEngine::GenericRequest rqt;
	AsyncNetworkEngine<GameWorkstore::AsyncNetworkEngine::GenericRequest, GameWorkstore::AsyncNetworkEngine::GenericRequest> MyAsyncNetworkEngineInstance;
	
	MyAsyncNetworkEngineInstance.Send(rqt);

	return true;
}

