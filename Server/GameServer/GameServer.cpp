#include "pch.h"
#include <thread>
#include <mutex>
#include <CoreMacro.h>
#include <ThreadManager.h>

//class TestLock
//{
//	USE_LOCK;
//	//shared_mutex _mutex;
//	
//public:
//	int32 testRead()
//	{
//		READ_LOCK;
//
//		//shared_lock<shared_mutex> lock(_mutex);
//		if (m_queue.empty())
//			return -1;
//
//		return m_queue.front();
//	}
//
//	void TestPush()
//	{
//		WRITE_LOCK;
//		testRead();
//		//unique_lock<shared_mutex>	lock(_mutex);
//		
//		m_queue.push(rand() % 100);
//
//	}
//	void TestPop()
//	{
//		WRITE_LOCK;
//		//unique_lock<shared_mutex>	lock(_mutex);
//		if (m_queue.empty() == false)
//			m_queue.pop();
//	}
//private:
//	queue<int32> m_queue;
//};
//
//TestLock testLock;
//
//void ThreadWrite()
//{
//	while (true)
//	{
//		testLock.TestPush();
//		this_thread::sleep_for(1s);
//		testLock.TestPop();
//	}
//}
//
//void ThreadRead()
//{
//	while (true)
//	{
//		int32 value = testLock.testRead();
//
//		cout << value << endl;
//		this_thread::sleep_for(1s);
//	}
//}

int main()
{   
	/*for (int32 i = 0; i < 30; ++i)
	{
		GThreadManager->Launch(ThreadWrite);
	}
	for (int32 i = 0; i < 20; ++i)
	{
		GThreadManager->Launch(ThreadRead);
	}
	GThreadManager->Join();*/
}