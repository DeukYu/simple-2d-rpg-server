#include "pch.h"
#include "DeadLockProfiler.h"

void DeadLockProfiler::PushLock(string_view name)
{
	lock_guard<mutex>	guard(mMutex);

	int32 lockId = 0;

	auto findIt = mNameToId.find(name);
	if (findIt == mNameToId.end())
	{
		lockId = static_cast<int32>(mNameToId.size());
		mNameToId[name] = lockId;
		mIdToName[lockId] = name;
	}
	else
	{
		lockId = findIt->second;
	}

	if (!mLockStack.empty() == false)
	{
		const int32 prevId = mLockStack.top();
		if (lockId != prevId)
		{
			set<int32>& history = mLockHistory[prevId];
			if (history.find(lockId) == history.end())
			{
				history.insert(lockId);
				CheckCycle();
			}
		}
	}
	mLockStack.push(lockId);
}

void DeadLockProfiler::PopLock(string_view name)
{
	lock_guard<mutex>	guard(mMutex);

	if (mLockStack.empty())
		ASSERT_CRASH("MULTIPLE_UNLOCK");

	int32 lockId = mNameToId[name];
	if (mLockStack.top() != lockId)
		ASSERT_CRASH("INVALID_UNLOCK");

	mLockStack.pop();
}

void DeadLockProfiler::CheckCycle()
{
	const int32 lockCount = static_cast<int32>(mNameToId.size());
	mDiscoveredOrder = vector<int32>(lockCount, -1);
	mDiscoveredCount = 0;
	mFinished = vector<bool>(lockCount, false);
	mParent = vector<int32>(lockCount, -1);

	for (int32 lockId = 0; lockId < lockCount; ++lockId)
		Dfs(lockId);

	mDiscoveredOrder.clear();
	mFinished.clear();
	mParent.clear();
}

void DeadLockProfiler::Dfs(int32 here)
{
	if (mDiscoveredOrder[here] != -1)
		return;

	mDiscoveredOrder[here] = mDiscoveredCount++;

	auto findIt = mLockHistory.find(here);
	if (findIt == mLockHistory.end())
	{
		mFinished[here] = true;
		return;
	}

	set<int32>& nextSet = findIt->second;
	for (int32 there : nextSet)
	{
		if (mDiscoveredOrder[there] == -1)
		{
			mParent[there] = here;
			Dfs(there);
			continue;
		}
		if (mDiscoveredOrder[here] < mDiscoveredOrder[there])
			continue;

		if (mFinished[there] == false)
		{
			std::cout << std::format("{} -> {}\n", mIdToName[here], mIdToName[there]);

			int32 now = here;
			while (true)
			{
				std::cout << std::format("{} -> {}\n", mIdToName[now], mIdToName[now]);
				now = mParent[now];
				if (now == there)
					break;
			}

			ASSERT_CRASH("DEADLOCK_DETECTED");
		}
	}
	mFinished[here] = true;
}
