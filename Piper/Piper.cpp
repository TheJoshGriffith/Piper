// Piper.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "Piper.h"
#include <Windows.h>
#include <Psapi.h>
#include <stdlib.h>

#define BUFFER_SIZE 512

Piper::Piper()
{
	// Get tibias main module for future use
	baseAddress = (DWORD) GetModuleHandle(0);
	//getModule();
	// Define a pipe name
	pipeName = L"\\\\.\\pipe\\piperpipe";
	// Create the pipe
	hPipe = CreateNamedPipe(pipeName, PIPE_ACCESS_DUPLEX, PIPE_TYPE_MESSAGE, 1, BUFFER_SIZE, BUFFER_SIZE, 5000, NULL);
	// If we didn't make it, tell the user something went wrong.
	if (hPipe == NULL || hPipe == INVALID_HANDLE_VALUE)
	{
		MessageBox(NULL, L"Error loading named pipe, please consult your software provider for support", L"Error!", MB_ICONSTOP);
	}
	// Wait for a client to connect to see the data down the pipe.
	ConnectNamedPipe(hPipe, NULL);
	// Send a message down the pipe to confirm it's working
	Send("QSYSTEM OPERATIONAL");
	// Sent the base address for good measure
	char *buff = new char[32];
	_itoa_s(baseAddress, buff, sizeof(buff), 16);
	Send(buff);
}

Piper::~Piper()
{
	// Clean up after ourselves by closing the pipe handle (and releasing it for future use)
	CloseHandle(hPipe);
}

void Piper::Send(char *msg)
{
	WriteFile(hPipe, msg, sizeof(msg), &lastPacketSize, NULL);
	char *buff = new char[32];
	_itoa_s(lastPacketSize, buff, sizeof(buff), 16);
	MessageBoxA(NULL, buff, buff, MB_ICONSTOP);
}

/*void Piper::getModule()
{
	DWORD cbNeeded;
	HANDLE pHandle = GetCurrentProcess();
	HMODULE hMods[1024];
	// Currently the first enumerated module will always be Tibia_exe.
	if (EnumProcessModules(pHandle, hMods, sizeof(hMods), &cbNeeded))
	{
		baseAddress = (DWORD) hMods[0];
	}
}*/