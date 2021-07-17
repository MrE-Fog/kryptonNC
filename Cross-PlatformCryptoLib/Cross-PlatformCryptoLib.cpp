﻿// Cross-PlatformCryptoLib.cpp : Defines the entry point for the application.
//
#include "Cross-PlatformCryptoLib.h"

#define PY_SSIZE_T_CLEAN

#include <Python.h>
#include <openssl/crypto.h>
#include <openssl/rand.h>
#include <tuple>
#include <openssl/aes.h>


std::tuple<char*, char> AESEncrypt(unsigned char* text, unsigned char* key) {
	int msglen = strlen((char*)text);
	unsigned char iv[8];

	RAND_bytes(iv, 8);
	unsigned char* out = new unsigned char[msglen];
	AES_KEY aes_key;
	AES_set_encrypt_key(key, 128, &aes_key);

	AES_cbc_encrypt(text, out, msglen, &aes_key, iv, AES_ENCRYPT);
	memset_s((void*)text, 0, strlen((const char*)text));
	memset_s((void*)key, 0, strlen((const char*)key));
	memset_s(&aes_key, 0, sizeof(aes_key));
	char* result = new char[msglen];
	memcpy(result, out, msglen);
	memset_s(out, 0, msglen);
	delete out;
	delete text;
	delete key;

	return { result, (char)iv };


}
char* AESDecrypt(unsigned char* iv, unsigned char* key, unsigned char* ctext) {
	int msglen = strlen((char*)ctext);
	unsigned char* out = new unsigned char[msglen];
	AES_KEY aes_key;
	AES_cbc_encrypt(ctext, out, msglen, &aes_key, iv, AES_DECRYPT);
	memset_s((void*)ctext, 0, strlen((const char*)ctext));
	memset_s((void*)key, 0, strlen((const char*)key));
	memset_s(&aes_key, 0, sizeof(aes_key));
	memset_s(&iv, 0, sizeof(iv));
	char* result = new char[msglen];
	memcpy(result, out, msglen);
	memset_s(out, 0, msglen);
	return result;
}

PyObject* AESEncryptPy(char* textb, char* keyb) {

	std::tuple<char*, char> a = AESEncrypt((unsigned char*)textb, (unsigned char*)keyb);
	PyObject* tup = Py_BuildValue("(yy)", std::get<0>(a), std::get<1>(a));
	memset_s(textb, 0, strlen(textb));
	memset_s(keyb, 0, strlen(keyb));
	delete keyb;
	delete textb;
	delete& a;

	return tup;
}


PyObject* AESDecryptPy(char* iv, char* key, char* ctext) {
	/*
	char* ctext = PyBytes_AsString(&ctextb);
	char* key = PyBytes_AsString(&keyb);
	char* iv = PyBytes_AsString(&ivb);
	*/
	char* a = AESDecrypt((unsigned char*)iv, (unsigned char*)key, (unsigned char*)ctext);  //We believe it is unecesary to delete arguments passed inside functions as it is passed as reference
	memset_s(key, 0, strlen(key));
	PyObject* result = Py_BuildValue("y", a);
	memset_s(a, 0, strlen(a));
	delete ctext;
	delete key;
	delete iv;
	delete a;
	return result;
}

void Init() {
	Py_Initialize();
	//Initialize();
}