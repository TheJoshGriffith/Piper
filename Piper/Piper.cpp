// Piper.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "Piper.h"

#define BUFFER_SIZE 512

Piper::Piper()
{
	pipeName = L"\\\\.\\pipe\\pipeName";
	hPipe = CreateNamedPipe(pipeName, PIPE_ACCESS_DUPLEX, PIPE_TYPE_MESSAGE, 1, BUFFER_SIZE, BUFFER_SIZE, 5000, NULL);
	if (hPipe == NULL || hPipe == INVALID_HANDLE_VALUE)
	{
		MessageBox(NULL, L"Error loading named pipe, please consult your software provider for support", L"Error!", MB_ICONSTOP);
	}
	ConnectNamedPipe(hPipe, NULL);
	wchar_t buff[BUFFER_SIZE] = L"SYSTEM OPERATIONAL!";
	WriteFile(hPipe, buff, sizeof(buff), &lastPacketSize, NULL);
}

Piper::~Piper()
{
	CloseHandle(hPipe);
}

void Piper::Send(wchar_t * msg)
{
	WriteFile(hPipe, msg, sizeof(msg), &lastPacketSize, NULL);
}