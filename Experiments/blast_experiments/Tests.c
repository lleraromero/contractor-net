#include <stdio.h>
#include <stdlib.h>

typedef int boolean;

int Estado;

boolean invariant() {
	return 1;
}

boolean sys_pre_Tests() {
	return 1;
}
boolean par_pre_Tests() {
	return 1;
}
void Tests() {
	Estado = 0;
}

boolean sys_pre_CicloMasLargoQueRecursionBound() {
	return Estado == 0;
}
boolean par_pre_CicloMasLargoQueRecursionBound() {
	return 1;
}
void CicloMasLargoQueRecursionBound()
{
	for (int i = 0; i < 10; i++)
	{
		Estado++;
	}
}

boolean sys_pre_CicloMasChicoQueRecursionBound() {
	return Estado == 0;
}
boolean par_pre_CicloMasChicoQueRecursionBound() {
	return 1;
}
void CicloMasChicoQueRecursionBound()
{
	for (int i = 0; i < 1; i++)
	{
		Estado++;
	}
}

boolean sys_pre_TestigoEstado1() {
	return Estado == 1;
}
boolean par_pre_TestigoEstado1() {
	return 1;
}
void TestigoEstado1()
{
}

boolean sys_pre_TestigoEstado10() {
	return Estado == 10;
}
boolean par_pre_TestigoEstado10() {
	return 1;
}
void TestigoEstado10()
{
}

boolean sys_pre_CicloBasadoEnParam() {
	return Estado == 0;
}
boolean par_pre_CicloBasadoEnParam() {
	return 1;
}
void CicloBasadoEnParam(int cota)
{
	for (int i = 0; i < cota; i++)
	{
		Estado++;
	}
}

boolean sys_pre_CicloDeberiaIrA10() {
	return Estado == 0;
}
boolean par_pre_CicloDeberiaIrA10() {
	return 1;
}
void CicloDeberiaIrA10()
{
	for (int i = 0; i < 30; i++)
	{
		Estado++;
	}
	Estado = 10;
}

boolean sys_pre_CicloNoDeberiaIrA10() {
	return Estado == 0;
}
boolean par_pre_CicloNoDeberiaIrA10() {
	return 1;
}
int CicloNoDeberiaIrA10()
{
	for (int i = 0; i < 30; i++)
	{
		return -1;
	}
	Estado = 10;
	return 0;
}

boolean sys_pre_CicloDeberiaIrA10ConIf() {
	return Estado == 0;
}
boolean par_pre_CicloDeberiaIrA10ConIf() {
	return 1;
}
void CicloDeberiaIrA10ConIf()
{
    for (int i = 0; i < 30; i++)
    {
        Estado++;
    }
    if (Estado == 30)
        Estado = 10;
}

boolean sys_pre_CicloNoDeberiaIrA10ConIf() {
	return Estado == 0;
}
boolean par_pre_CicloNoDeberiaIrA10ConIf() {
	return 1;
}
void CicloNoDeberiaIrA10ConIf()
{
    for (int i = 0; i < 30; i++)
    {
        if (i == 10)
            return;
    }
    Estado = 10;
}
