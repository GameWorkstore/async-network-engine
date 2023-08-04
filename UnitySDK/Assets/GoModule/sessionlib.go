package asyncnetworkengine

import (
	"errors"
	"time"

	"github.com/golang-jwt/jwt/v5"
	"google.golang.org/protobuf/proto"
)

var jwtEnabled = false
var jwtMethod jwt.SigningMethod
var jwtIssuer = ""
var jwtKey interface{} = nil
var jwtMaxAge time.Duration

// SetupToken setup function to emit and validate tokens on behalf of the user
// in doubt about maxAge, use time.Hour
func SetupToken(method jwt.SigningMethod, issuer string, key interface{}, maxAge time.Duration) {
	jwtIssuer = issuer
	jwtKey = key
	jwtMethod = method
	jwtMaxAge = maxAge
	jwtEnabled = true
}

func SetupHS256Token(issuer string, key string, maxAge time.Duration) {
	SetupToken(jwt.SigningMethodHS256, issuer, []byte(key), maxAge)
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

	var claims jwt.RegisteredClaims
	claims.Issuer = jwtIssuer
	claims.ID = string(user)
	claims.IssuedAt = jwt.NewNumericDate(time.Now())
	claims.ExpiresAt = jwt.NewNumericDate(time.Now().Add(jwtMaxAge))
	token, err := jwt.NewWithClaims(jwtMethod, claims).SignedString(jwtKey)
	if err != nil {
		return "", err
	}

	return token, nil
}

// ValidateToken validates a token return the user structure
func ValidateToken(sessionToken string, pb proto.Message) error {
	if !jwtEnabled {
		return errors.New("required to SetupToken once before use it")
	}

	token, err := jwt.ParseWithClaims(sessionToken, &jwt.RegisteredClaims{}, jwtKeyFunction)
	if err != nil || !token.Valid {
		return err
	}

	claims, castSuccess := token.Claims.(*jwt.RegisteredClaims)
	if !castSuccess {
		return errors.New("cast to StandardClaims failed")
	}

	err = proto.Unmarshal([]byte(claims.ID), pb)
	if err != nil {
		return err
	}

	return nil
}

func jwtKeyFunction(token *jwt.Token) (interface{}, error) { return jwtKey, nil }
