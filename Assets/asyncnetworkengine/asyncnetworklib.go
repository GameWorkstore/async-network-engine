package asyncnetworkengine

import (
	"io/ioutil"
	"net/http"

	"encoding/base64"

	"github.com/aws/aws-lambda-go/events"
	"github.com/golang/protobuf/proto"
)

var allowedCORS = false
var allowedCredentials = "true"
var allowedOrigin = "*"
var allowedHeaders = "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, Content-Type"
var allowedMethods = "POST"
var allowedMaxAge = "3600"

// EnableCORS setup default CORS to enabled.
func EnableCORS() {
	allowedCORS = true
}

// SetupCORS enable CORS and configures AsyncNetworkEngine to accept methods, headers and etc from a given application.
// For more information about CORS and CORS preflight requests, see
// https://developer.mozilla.org/en-US/docs/Glossary/Preflight_request.
func SetupCORS(allowCredentials bool, allowOrigin string, allowHeaders string, allowMethods string, maxAge int) {
	allowedCORS = true
	if allowCredentials {
		allowedCredentials = "true"
	} else {
		allowedCredentials = "false"
	}
	allowedOrigin = allowOrigin
	allowedHeaders = allowHeaders
	allowedMethods = allowMethods
	allowedMaxAge = string(maxAge)
}

func setupAWSCORS(w *events.APIGatewayProxyResponse) {
	w.Headers = make(map[string]string)
	w.Headers["Access-Control-Allow-Credentials"] = allowedCredentials
	w.Headers["Access-Control-Allow-Origin"] = allowedOrigin
	w.Headers["Access-Control-Allow-Headers"] = allowedHeaders
	w.Headers["Access-Control-Allow-Methods"] = allowedMethods
}

// GCPDecode decodes and returns the protobuf of given connection. returns
func GCPDecode(r http.Request, w http.ResponseWriter, rqt proto.Message) bool {

	if allowedCORS {
		w.Header().Set("Access-Control-Allow-Credentials", allowedCredentials)
		w.Header().Set("Access-Control-Allow-Origin", allowedOrigin)
		w.Header().Set("Access-Control-Allow-Headers", allowedHeaders)
		w.Header().Set("Access-Control-Allow-Methods", allowedMethods)
		w.Header().Set("Access-Control-Max-Age", "3600")
	}

	//stops immediatelly at OPTIONS
	if r.Method == http.MethodOptions && allowedCORS {
		w.WriteHeader(int(Transmission_Ok))
		return false
	}

	if r.Method != http.MethodPost {
		w.WriteHeader(int(Transmission_NotAllowed))
		return false
	}

	data, err := ioutil.ReadAll(r.Body)
	if err != nil {
		GCPError(w, Transmission_MarshalDecodeError, nil)
		return false
	}

	err = proto.Unmarshal(data, rqt)
	if err != nil {
		GCPError(w, Transmission_MarshalDecodeError, nil)
		return false
	}
	return true
}

// GCPResponse writes response in given connection.
func GCPResponse(w http.ResponseWriter, pb proto.Message) {
	data, err := proto.Marshal(pb)
	if err != nil {
		GCPError(w, Transmission_MarshalEncodeError, err)
		return
	}

	_, errw := w.Write(data)
	if errw != nil {
		GCPError(w, Transmission_ResponseWrite, err)
		return
	}
	// body before header success
	w.WriteHeader(int(Transmission_Ok))
}

// GCPError writes an error response in given connection.
func GCPError(w http.ResponseWriter, status Transmission, err error) {
	//try to send the error:
	var gerr GenericErrorResponse
	gerr.Error = err.Error()
	data, _ := proto.Marshal(&gerr)
	w.Write(data)
	w.WriteHeader(int(status))
}

// AWSDecode decodes and returns the protobuf of given connection.
func AWSDecode(r events.APIGatewayProxyRequest, w *events.APIGatewayProxyRequest, rqt proto.Message) (bool, events.APIGatewayProxyResponse, error) {
	resp := events.APIGatewayProxyResponse{}
	if allowedCORS {
		resp.Headers = make(map[string]string)
		resp.Headers["Access-Control-Allow-Credentials"] = allowedCredentials
		resp.Headers["Access-Control-Allow-Origin"] = allowedOrigin
		resp.Headers["Access-Control-Allow-Headers"] = allowedHeaders
		resp.Headers["Access-Control-Allow-Methods"] = allowedMethods
	}

	if r.HTTPMethod == "OPTIONS" {
		resp.StatusCode = int(Transmission_Ok)
		return false, resp, nil
	}

	data, err := base64.URLEncoding.DecodeString(request.Body)
	if err != nil {
		return err
	}
	err = proto.Unmarshal(data, rqt)
	if err != nil {
		return err
	}
	return nil
}

// AWSResponse writes response in given connection.
func AWSResponse(pb proto.Message) (events.APIGatewayProxyResponse, error) {
	data, err := proto.Marshal(pb)
	if err != nil {
		return AWSError(transmissionMarshalEncodeError, err)
	}
	resp := events.APIGatewayProxyResponse{}
	resp.Body = base64.URLEncoding.EncodeToString(data)
	resp.StatusCode = transmissionSuccess
	setupAWSCORS(&resp)
	return resp, nil
}

// AWSError writes an error response in given connection.
func AWSError(status Transmission, err error) (events.APIGatewayProxyResponse, error) {
	var gerr GenericErrorResponse
	gerr.Status = int32(status)
	if err != nil {
		gerr.Error = err.Error()
	} else {
		gerr.Error = "nil"
	}
	data, _ := proto.Marshal(&gerr)

	resp := events.APIGatewayProxyResponse{}
	resp.Body = base64.URLEncoding.EncodeToString(data)
	resp.StatusCode = status
	enableAWSCORS(&resp)
	return resp, nil
}
