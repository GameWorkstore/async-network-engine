// Fill out your copyright notice in the Description page of Project Settings.
#include "CoreMinimal.h"
#include "Misc/AutomationTest.h"
#include "ase/asyncrpc.pb.h"
#include "AsyncNetworkEngine.h"

IMPLEMENT_SIMPLE_AUTOMATION_TEST(FASEGoogleProtobufTest, "ASE.GoogleProtobufTest", EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

bool FASEGoogleProtobufTest::RunTest(const FString& Parameters)
{
	const std::string msg = "Hello World";
	// init
	GameWorkstore::AsyncNetworkEngine::GenericRequest rqt;
	rqt.set_messege(msg.c_str());
	// process
	TArray<uint8> buffer;
	buffer.SetNumUninitialized(rqt.ByteSizeLong(), false);
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


IMPLEMENT_SIMPLE_AUTOMATION_TEST(FASESend, "ASE.SendTest", EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

static void OnResponseReceived(GameWorkstore::AsyncNetworkEngine::GenericResponse Response)
{
	UE_LOG(LogTemp, Warning, TEXT("Tamanho da resposta: %d"), Response.ByteSizeLong());

	//UE_LOG(LogTemp, Warning, TEXT("Minha string: %s"), Response.messege();
}

bool FASESend::RunTest(const FString& Parameters)
{
	GameWorkstore::AsyncNetworkEngine::GenericRequest rqt;
	//AsyncNetworkEngineTest test;
	rqt.set_messege("message test");
	AsyncNetworkEngine<GameWorkstore::AsyncNetworkEngine::GenericRequest, GameWorkstore::AsyncNetworkEngine::GenericResponse>::Send(
		"https://phy-dev-api.phyengine.com/phy-dev-gettest", rqt, &OnResponseReceived);
	return false;
}

