#include "pch.h"
#include <thread>
#include <mutex>

vector<int32> v;
mutex m;

void Push()
{
	for (int32 i = 0; i < 100'000; ++i)
	{
		m.lock();
		v.emplace_back(i);
		m.unlock();
	}
}

int main()
{   
	thread t1(Push);
	thread t2(Push);

	t1.join();
	t2.join();
}