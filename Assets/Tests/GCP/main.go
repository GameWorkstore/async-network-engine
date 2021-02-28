package main

import (
	"net/http"

	ase "github.com/GameWorkstore/async-network-engine/Assets/asyncnetworkengine"
)

// Process handler of this test
func Process(w http.ResponseWriter, r *http.Request) {
	//var resp ase.GenericMessegeResponse
	ase.GCPDecode(r, w, nil)
}
