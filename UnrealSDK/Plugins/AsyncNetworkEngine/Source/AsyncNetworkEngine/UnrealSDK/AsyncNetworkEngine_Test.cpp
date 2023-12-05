// Fill out your copyright notice in the Description page of Project Settings.
#include "CoreMinimal.h"
#include "Misc/AutomationTest.h"
#include "ase/asyncrpc.pb.h"
#include "ase/asyncnetworkengine.h"
#include "Logging/StructuredLog.h"

using namespace GameWorkstore::AsyncNetworkEngine;

IMPLEMENT_SIMPLE_AUTOMATION_TEST(FASEGoogleProtobufTest, "ASE.GoogleProtobufTest", EAutomationTestFlags::EditorContext | EAutomationTestFlags::ProductFilter)

bool FASEGoogleProtobufTest::RunTest(const FString& Parameters)
{
	const std::string msg = "Hello World";
	// init
	GenericRequest rqt;
	rqt.set_messege(msg.c_str());
	// process
	TArray<uint8> buffer;
	buffer.SetNumUninitialized(rqt.ByteSizeLong(), false);
	rqt.SerializeToArray(buffer.GetData(), rqt.ByteSizeLong());
	GenericRequest receiver;
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

static void OnResponseReceived(Transmission transmission, GenericResponse resp, GenericErrorResponse error)
{
	UE_LOGFMT(LogTemp, Warning, "Transmission: {0}", static_cast<int>(transmission));
	UE_LOGFMT(LogTemp, Warning, "Tamanho da resposta: {0}", static_cast<int>(resp.ByteSizeLong()));
	UE_LOGFMT(LogTemp, Warning, "Minha string: {0}", FString(resp.messege().c_str()));
}

bool FASESend::RunTest(const FString& Parameters)
{
	GenericRequest rqt;
	//AsyncNetworkEngineTest test;
	rqt.set_messege("message test");

	AsyncNetworkEngine<GenericRequest, GenericResponse>::FAsyncNetworkCallback callback;
	callback.BindStatic(OnResponseReceived);
	AsyncNetworkEngine<GenericRequest, GenericResponse>::Send("https://phy-dev-api.phyengine.com/phy-dev-gettest", rqt, callback);
	return false;
}

