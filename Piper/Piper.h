#include <Windows.h>

class Piper
{
public:
	// Methods
	Piper();
	~Piper();
	void Send(char * msg);

	// Variables
	HANDLE hPipe;
	wchar_t *pipeName;
	DWORD lastPacketSize;
	DWORD baseAddress;
};