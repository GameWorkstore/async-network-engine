package gcptesting

import (
	"errors"
	"fmt"
	"net/http"

	ase "github.com/GameWorkstore/async-network-engine-go"
)

func init() {
	ase.EnableCORS()
}

// Process handler of this test
func Process(w http.ResponseWriter, r *http.Request) {

	fmt.Println("T")

	rqt := ase.GenericRequest{}
	if ase.GCPDecode(r, w, &rqt) {
		return
	}

	fmt.Println("M:" + rqt.Messege)

	switch rqt.Messege {
	case "decode-error":
		ase.GCPError(w, ase.Transmission_MarshalDecodeError, errors.New("decode error"))
		break
	case "encode-error":
		ase.GCPError(w, ase.Transmission_MarshalEncodeError, errors.New("encode error"))
		break
	case "internal-error":
		ase.GCPError(w, ase.Transmission_InternalHandlerError, errors.New("internal error"))
		break
	case "not-allowed-error":
		ase.GCPError(w, ase.Transmission_NotAllowed, errors.New("internal error"))
		break
	case "not-implemented":
		ase.GCPError(w, ase.Transmission_NotImplemented, errors.New("internal error"))
		break
	}

	resp := ase.GenericResponse{}
	resp.Messege = "received-" + rqt.Messege
	ase.GCPResponse(w, &resp)
}
