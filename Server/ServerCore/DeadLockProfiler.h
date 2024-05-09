#pragma once

class DeadLockProfiler
{
public:
	void PushLock(string_view name);
	void PopLock(string_view name);
	void CheckCycle();

private:
	void Dfs(int32 here);

private:
	unordered_map<string_view, int32>	mNameToId;
	unordered_map<int32, string_view>	mIdToName;
	stack<int32>						mLockStack;
	map<int32, set<int32>>				mLockHistory;

	mutex	mMutex;

private:
	vector<int32>	mDiscoveredOrder;		// 노드가 발견된 순서를 기록하는 배열
	int32			mDiscoveredCount = 0;	// 노드가 발견된 순서
	vector<bool>	mFinished;				// Dfs(i)가 종료되었는지 여부
	vector<int32>	mParent;
};

