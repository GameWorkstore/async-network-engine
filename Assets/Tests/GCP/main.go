package gcptesting

import (
	"errors"
	"net/http"

	ase "github.com/GameWorkstore/async-network-engine-go"
)

func init() {
	ase.EnableCORS()
}

// Process handler of this test
func Process(w http.ResponseWriter, r *http.Request) {

	rqt := ase.GenericRequest{}
	if ase.GCPDecode(r, w, &rqt) {
		return
	}

	switch rqt.Messege {
	case "decode-error":
		ase.GCPError(w, ase.Transmission_ErrorDecode, errors.New("decode error"))
		return
	case "encode-error":
		ase.GCPError(w, ase.Transmission_ErrorEncode, errors.New("encode error"))
		return
	case "internal-error":
		ase.GCPError(w, ase.Transmission_ErrorInternalServer, errors.New("internal error"))
		return
	case "not-allowed-error":
		ase.GCPError(w, ase.Transmission_ErrorMethodNotAllowed, errors.New("not allowed error"))
		return
	case "not-implemented":
		ase.GCPError(w, ase.Transmission_ErrorNotImplemented, errors.New("not implemented"))
		return
	}

	resp := ase.GenericResponse{}
	resp.Messege = "received-" + rqt.Messege
	ase.GCPResponse(w, &resp)
}
