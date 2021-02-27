package main

import (
	"io/ioutil"
	"net/http"

	"encoding/base64"

	"github.com/aws/aws-lambda-go/events"
	"github.com/golang/protobuf/proto"
)

//default errors:
const transmissionSuccess = 200
const transmissionMarshalDecodeError = 290
const transmissionMarshalEncodeError = 291
const transmissionResponseWrite = 292
const transmissionNotImplemented = 293
const transmissionInternalHandlerError = 294

////// Requests and Responses
// CORSEnabledFunction is an example of setting CORS headers.
// For more information about CORS and CORS preflight requests, see
// https://developer.mozilla.org/en-US/docs/Glossary/Preflight_request.
func enableGoogleCORS(w http.ResponseWriter) {
	// Set CORS headers for the preflight request
	w.Header().Set("Access-Control-Allow-Credentials", "true")
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, Content-Type")
	w.Header().Set("Access-Control-Allow-Methods", "GET, POST")
}

func enableAWSCORS(w *events.APIGatewayProxyResponse) {
	w.Headers = make(map[string]string)
	w.Headers["Access-Control-Allow-Origin"] = "*"
	w.Headers["Access-Control-Allow-Headers"] = "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time, Content-Type"
	w.Headers["Access-Control-Allow-Methods"] = "GET, POST"
	w.Headers["Access-Control-Allow-Credentials"] = "true"
}

func googleDecode(r *http.Request, rqt proto.Message) error {
	data, err := ioutil.ReadAll(r.Body)
	if err != nil {
		return err
	}

	err = proto.Unmarshal(data, rqt)
	if err != nil {
		return err
	}
	return nil
}

func googleResponse(w http.ResponseWriter, pb proto.Message) {
	data, err := proto.Marshal(pb)
	if err != nil {
		googleError(w, transmissionMarshalEncodeError, err)
		return
	}
	_, errw := w.Write(data)
	if errw != nil {
		googleError(w, transmissionResponseWrite, err)
		return
	}
	w.WriteHeader(transmissionSuccess)
}

func googleError(w http.ResponseWriter, status int, err error) {
	w.WriteHeader(status)
	//try to send the error:
	var gerr GenericErrorResponse
	gerr.Status = int32(status)
	gerr.Error = err.Error()
	data, _ := proto.Marshal(&gerr)
	w.Write(data)
}

func gatewayDecode(request events.APIGatewayProxyRequest, rqt proto.Message) error {
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

func gatewayResponse(pb proto.Message) (events.APIGatewayProxyResponse, error) {
	data, err := proto.Marshal(pb)
	if err != nil {
		return gatewayError(transmissionMarshalEncodeError, err)
	}
	resp := events.APIGatewayProxyResponse{}
	resp.Body = base64.URLEncoding.EncodeToString(data)
	resp.StatusCode = transmissionSuccess
	enableAWSCORS(&resp)
	return resp, nil
}

func gatewayError(status int, err error) (events.APIGatewayProxyResponse, error) {
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
