// Fill out your copyright notice in the Description page of Project Settings.
#include "AsyncNetworkEngineGameMode.h"
#include "CoreMinimal.h"
#include "Misc/AutomationTest.h"
#include "Proto/asyncrpc.pb.h"

IMPLEMENT_SIMPLE_AUTOMATION_TEST(FASEGoogleProtobufTest, "ASE.GoogleProtobufTest", EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

// Your function must be named RunTest
// The struct name here "FHeroTest" must match the one in the macro above
bool FASEGoogleProtobufTest::RunTest(const FString& Parameters)
{
	const std::string msg = "Hello World";

	// init

	GameWorkstore::AsyncNetworkEngine::GenericRequest rqt;
	rqt.set_messege(msg.c_str());

	// process

	TArray<uint8> buffer;
	buffer.SetNumUninitialized(rqt.ByteSizeLong(),false);
	rqt.SerializeToArray(buffer.GetData(), rqt.ByteSizeLong());

	GameWorkstore::AsyncNetworkEngine::GenericRequest receiver;
	receiver.ParseFromArray(buffer.GetData(), buffer.Num());

	std::string rcv = receiver.messege();

	// assert
	if (msg.compare(rcv) != 0)
	{
		AddError(TEXT("strings are different!"));
		return false;
	}

	return true;
}

