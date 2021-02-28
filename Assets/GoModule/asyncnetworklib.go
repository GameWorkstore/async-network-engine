package main

import (
	"io/ioutil"
	"net/http"
	"strconv"

	"encoding/base64"

	"github.com/aws/aws-lambda-go/events"
	"google.golang.org/protobuf/proto"
)

var allowedCORS = false
var allowedCredentials = "true"
var allowedOrigin = "*"
var allowedHeaders = "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, Content-Type"
var allowedMethods = "POST"
var allowedMaxAge = "3600"

func main() {

}

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
	allowedMaxAge = strconv.Itoa(maxAge)
}

// GCPDecode decodes and returns the protobuf of given connection. returns true if break by OPTIONS or error.
func GCPDecode(r *http.Request, w http.ResponseWriter, rqt proto.Message) bool {

	if allowedCORS {
		w.Header().Set("Access-Control-Allow-Credentials", allowedCredentials)
		w.Header().Set("Access-Control-Allow-Origin", allowedOrigin)
		w.Header().Set("Access-Control-Allow-Headers", allowedHeaders)
		w.Header().Set("Access-Control-Allow-Methods", allowedMethods)
		w.Header().Set("Access-Control-Max-Age", "3600")
	}

	//stops immediatelly at OPTIONS
	if r.Method == http.MethodOptions {
		if allowedCORS {
			w.WriteHeader(int(Transmission_Ok))
		} else {
			w.WriteHeader(int(Transmission_NotAllowed))
		}
		return true
	}

	if r.Method != http.MethodPost {
		w.WriteHeader(int(Transmission_NotAllowed))
		return true
	}

	data, err := ioutil.ReadAll(r.Body)
	if err != nil {
		GCPError(w, Transmission_MarshalDecodeError, nil)
		return true
	}

	err = proto.Unmarshal(data, rqt)
	if err != nil {
		GCPError(w, Transmission_MarshalDecodeError, nil)
		return true
	}
	return false
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
	if err != nil {
		var gerr GenericErrorResponse
		gerr.Error = err.Error()
		data, _ := proto.Marshal(&gerr)
		w.Write(data)
	}
	w.WriteHeader(int(status))
}

// AWSPreFlight should be called before AWSDecode. if method is OPTIONS, returns true. Return to gateway (response,nil) if true.
func AWSPreFlight(r events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, bool) {
	w := events.APIGatewayProxyResponse{}
	switch r.HTTPMethod {
	case "OPTIONS":
		if allowedCORS {
			w := events.APIGatewayProxyResponse{}
			w.Headers = make(map[string]string)
			w.Headers["Access-Control-Allow-Credentials"] = allowedCredentials
			w.Headers["Access-Control-Allow-Origin"] = allowedOrigin
			w.Headers["Access-Control-Allow-Headers"] = allowedHeaders
			w.Headers["Access-Control-Allow-Methods"] = allowedMethods
			w.Headers["Access-Control-Max-Age"] = allowedMaxAge
			w.StatusCode = int(Transmission_Ok)
		} else {
			w.StatusCode = int(Transmission_NotAllowed)
		}
		return w, true
	case "POST":
		//passthrough
		return w, false
	}
	w.StatusCode = int(Transmission_NotAllowed)
	return w, true
}

// AWSDecode decodes and returns the protobuf of given connection. If err != nil, return an AWSError.
func AWSDecode(r events.APIGatewayProxyRequest, rqt proto.Message) error {
	data, err := base64.URLEncoding.DecodeString(r.Body)
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
		return AWSError(Transmission_MarshalEncodeError, err)
	}
	w := events.APIGatewayProxyResponse{}
	w.Body = base64.URLEncoding.EncodeToString(data)
	w.StatusCode = int(Transmission_Ok)
	return w, nil
}

// AWSError writes an error response in given connection.
func AWSError(status Transmission, err error) (events.APIGatewayProxyResponse, error) {
	var gerr GenericErrorResponse
	if err != nil {
		gerr.Error = err.Error()
	} else {
		gerr.Error = "null"
	}
	data, _ := proto.Marshal(&gerr)
	w := events.APIGatewayProxyResponse{}
	w.Body = base64.URLEncoding.EncodeToString(data)
	w.StatusCode = int(status)
	return w, nil
}
