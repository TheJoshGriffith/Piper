#include <Windows.h>

class Piper
{
public:
	// Methods
	Piper();
	~Piper();
	void Send(wchar_t * msg);
	//void getModule();

	// Variables
	HANDLE hPipe;
	wchar_t *pipeName;
	DWORD lastPacketSize;
	DWORD baseAddress;
};