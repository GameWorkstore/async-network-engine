package asyncnetworkengine

import (
	"testing"
	"time"
)

func TestValidateToken(t *testing.T) {

	SetupHS256Token("myissuer", "sa68oa798ydYA&*TuhayugaA¨&AS5aS¨&%8AS6yATg", 1*time.Hour)

	var rqt GenericRequest
	rqt.Messege = "HelloWorld"
	token, err := CreateToken(&rqt)
	if err != nil {
		t.Error("token creation failed", err)
	}

	var rec GenericRequest
	err = ValidateToken(token, &rec)
	if err != nil {
		t.Error("token creation failed", err)
	}

	if rec.Messege != rqt.Messege {
		t.Error("token mismatch", err)
	}

	t.Log("success:", rec.Messege)
}
