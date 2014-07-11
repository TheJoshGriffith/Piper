#include <Windows.h>

class Piper
{
public:
	// Methods
	Piper();
	~Piper();
	void Send(char * msg);
	//void getModule();

	// Variables
	HANDLE hPipe;
	wchar_t *pipeName;
	DWORD lastPacketSize;
	DWORD baseAddress;
};