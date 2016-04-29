#include <stdlib.h>
#include <stdio.h>
#include <string.h>


// Type redefinitions

typedef int boolean;
typedef char byte;
typedef int PublicKey;
typedef int PrivateKey;


// SignatureSpi

void engineInitVerify(PublicKey publicKey) {
	// hardcoded (due to abstract class)
}

void engineInitSign(PrivateKey privateKey) {
	// hardcoded (due to abstract class)
}

void engineUpdate(byte b) {
	// hardcoded (due to abstract class)
}

byte* engineSign() {
	// hardcoded result (due to abstract class)
	byte* ret = calloc(10, sizeof(byte)); 
	return ret;
}

boolean engineVerify(byte* sigBytes) {
	// hardcoded result (due to abstract class)
	return 1;
}

// Signature

int UNINITIALIZED = 0;              
int SIGN = 2;
int VERIFY = 3;

int S_state;

boolean invariant() {
	return S_state == UNINITIALIZED || S_state == SIGN || S_state == VERIFY;
}

boolean sys_pre_Signature() {
	return 1;
}
boolean par_pre_Signature() {
	return 1;
}
void Signature() {
	S_state = UNINITIALIZED;
}

boolean sys_pre_initVerify() {
	return 1;
}
boolean par_pre_initVerify(PublicKey publicKey) {
	return 1;
}
void initVerify(PublicKey publicKey) { // throws InvalidKeyException {
	engineInitVerify(publicKey);
	S_state = VERIFY;
}

boolean sys_pre_initSign() {
	return 1;
}
boolean par_pre_initSign(PrivateKey privateKey) {
	return 1;
}
void initSign(PrivateKey privateKey) { // throws InvalidKeyException {
	engineInitSign(privateKey);
	S_state = SIGN;
}

boolean sys_pre_sign() {
	return S_state == SIGN;
}
boolean par_pre_sign() {
	return 1;
}
byte* sign() { // throws SignatureException {
	//if (S_state == SIGN) {
		return engineSign();
	//}
	//throw new SignatureException("object not initialized for " + "signing");
}

boolean sys_pre_verify() {
	return S_state == VERIFY;
}
boolean par_pre_verify(byte* signature) {
	return 1;
}
boolean verify(byte* signature) { // throws SignatureException {
	//if (S_state == VERIFY) {
		return engineVerify(signature);
	//}
	//throw new SignatureException("object not initialized for " + "verification");
}

boolean sys_pre_update() {
	return S_state == VERIFY || S_state == SIGN;
}
boolean par_pre_update(byte b) {
	return 1;
}
void update(byte b) { // throws SignatureException {
	//if (S_state == VERIFY || S_state == SIGN) {
		engineUpdate(b);
	//} else {
	//	throw new SignatureException("object not initialized for " + "signature or verification");
	//}
}
