package main

import (
	"context"
	"errors"
	"fmt"
	"os"

	ase "github.com/GameWorkstore/async-network-engine-go"
	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
)

// NotImplemented returns not implemented for who called it
func NotImplemented(ctx context.Context, request events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, error) {
	return ase.AWSError(ase.Transmission_ErrorNotImplemented, errors.New("not implemented"))
}

// Process is a sample function
func Process(ctx context.Context, request events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, error) {

	// Cross-Origin Domain - WebGL
	rqt := ase.GenericRequest{}
	respOptions, shouldStop := ase.AWSDecode(request, &rqt)
	if shouldStop != nil {
		return respOptions, nil
	}

	switch rqt.Messege {
	case "decode-error":
		return ase.AWSError(ase.Transmission_ErrorDecode, errors.New("decode error"))
	case "encode-error":
		return ase.AWSError(ase.Transmission_ErrorEncode, errors.New("encode error"))
	case "internal-error":
		return ase.AWSError(ase.Transmission_ErrorInternalServer, errors.New("internal error"))
	case "not-allowed-error":
		return ase.AWSError(ase.Transmission_ErrorMethodNotAllowed, errors.New("not allowed error"))
	case "not-implemented":
		return ase.AWSError(ase.Transmission_ErrorNotImplemented, errors.New("not implemented"))
	}

	resp := ase.GenericResponse{}
	resp.Messege = "received-" + rqt.Messege
	return ase.AWSResponse(&resp)
}

// BinaryConversion receives, decodes encodes and send to test if conversion are working properly.
func BinaryConversion(ctx context.Context, request events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, error) {

	rqt := ase.GenericRequest{}
	respOptions, shouldStop := ase.AWSDecode(request, &rqt)
	if shouldStop != nil {
		return respOptions, nil
	}

	return ase.AWSResponse(&rqt)
}

func init() {
	ase.EnableCORS()
}

func main() {
	var functionName = os.Getenv("AWS_LAMBDA_FUNCTION_NAME")
	fmt.Println(functionName)
	switch functionName {
	case "aseawstest":
		lambda.Start(Process)
		return
	case "asebinaryconversions":
		lambda.Start(BinaryConversion)
		return
	default:
		lambda.Start(NotImplemented)
		return
	}
}
