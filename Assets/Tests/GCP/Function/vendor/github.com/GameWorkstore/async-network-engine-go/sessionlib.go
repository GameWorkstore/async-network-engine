package asyncnetworkengine

import (
	"errors"
	"strings"
	"time"

	"github.com/dgrijalva/jwt-go"
	"google.golang.org/protobuf/proto"
)

var jwtEnabled = false
var jwtMethod jwt.SigningMethod
var jwtIssuer = "asyncnetworkengine"
var jwtHeader = ""
var jwtKey = []byte("null")
var jwtMaxAge time.Duration

// SetupToken setup function to emit and validate tokens on behalf of the user
// in doubt about maxAge, use time.Hour
func SetupToken(issuer string, key string, maxAge time.Duration) {
	jwtIssuer = issuer
	jwtKey = []byte(key)
	jwtMethod = jwt.SigningMethodHS256
	jwtHeader = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
	jwtMaxAge = maxAge
	jwtEnabled = true
}

// CreateToken creates a token for an given user
func CreateToken(pb proto.Message) (string, error) {

	if !jwtEnabled {
		return "", errors.New("required to SetupToken once before use it")
	}

	user, err := proto.Marshal(pb)
	if err != nil {
		return "", err
	}

	var claims jwt.StandardClaims
	claims.Issuer = jwtIssuer
	claims.Id = string(user)
	claims.IssuedAt = jwt.TimeFunc().Unix()
	claims.ExpiresAt = jwt.TimeFunc().Add(jwtMaxAge).Unix()
	token, err := jwt.NewWithClaims(jwt.SigningMethodHS256, claims).SignedString(jwtKey)
	if err != nil {
		return "", err
	}

	return strings.TrimPrefix(token, jwtHeader), nil
}

// ValidateToken validates a token return the user structure
func ValidateToken(sessionToken string, pb proto.Message) error {
	if !jwtEnabled {
		return errors.New("required to SetupToken once before use it")
	}

	token, err := jwt.ParseWithClaims(jwtHeader+sessionToken, &jwt.StandardClaims{}, jwtKeyFunction)
	if err != nil || !token.Valid {
		return err
	}

	claims, castSuccess := token.Claims.(*jwt.StandardClaims)
	if !castSuccess {
		return errors.New("cast to StandardClaims failed")
	}

	err = proto.Unmarshal([]byte(claims.Id), pb)
	if err != nil {
		return err
	}

	return nil
}

func jwtKeyFunction(token *jwt.Token) (interface{}, error) { return jwtKey, nil }
