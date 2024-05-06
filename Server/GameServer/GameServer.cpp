#include "pch.h"
#include <thread>
#include <mutex>
#include <CoreMacro.h>
#include <ThreadManager.h>

CoreGlobal Core;

void ThreadMain()
{
	while (true)
	{
		cout << " Hello Thread: " << LThreadId << endl;
		this_thread::sleep_for(1s);
	}
}

int main()
{   
	/*int32 a = 3;
	ASSERT_CRASH(a != 3);*/
	for (int32 i = 0; i < 5; ++i)
	{
		GThreadManager->Launch(ThreadMain);
	}
	GThreadManager->Join();
}