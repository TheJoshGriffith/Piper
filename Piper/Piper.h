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
	static HANDLE hPipe;
	static wchar_t *pipeName;
	DWORD lastPacketSize;
	DWORD baseAddress;
};