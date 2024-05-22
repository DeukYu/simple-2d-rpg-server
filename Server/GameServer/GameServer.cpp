#include "pch.h"
#include <thread>
#include <mutex>
#include <CoreMacro.h>
#include <ThreadManager.h>

class Knight
{
public:
	Knight() {
		cout << "Knight()" << endl;
	}
	~Knight() {
		cout << "~Knight()" << endl;
	}
};

int main()
{   
	shared_ptr<Knight> spr = make_shared<Knight>();
}
