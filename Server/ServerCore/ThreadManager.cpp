#include "pch.h"
#include "ThreadManager.h"
#include "CoreTLS.h"
#include "CoreGlobal.h"

ThreadManager::ThreadManager()
{
	InitTLS();
}

ThreadManager::~ThreadManager()
{
	Join();
}

void ThreadManager::Launch(function<void(void)> callback)
{
	lock_guard<mutex> guard(mMutex);

	mThreads.emplace_back([=]() {
		InitTLS();
		callback();
		DestroyTLS();
		});
}

void ThreadManager::Join()
{
	for (thread& t : mThreads)
	{
		if (t.joinable())
			t.join();
	}
}

void ThreadManager::InitTLS()
{
	static atomic<uint32> SThreadId = 1;
	LThreadId = SThreadId.fetch_add(1);
}

void ThreadManager::DestroyTLS()
{
}
