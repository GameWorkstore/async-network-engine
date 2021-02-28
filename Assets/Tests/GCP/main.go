package main

import (
	"net/http"

	ase "github.com/GameWorkstore/async-network-engine-go"
)

// Process handler of this test
func Process(w http.ResponseWriter, r *http.Request) {
	rqt := ase.GenericRequest{}
	ase.GCPDecode(r, w, &rqt)
}
