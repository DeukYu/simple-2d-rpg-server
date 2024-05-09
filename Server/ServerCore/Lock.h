#pragma once
#include "Types.h"

class Lock
{
public:
	void	WriteLock(string_view name);
	void	WriteUnlock(string_view name);
	void	ReadLock(string_view name);
	void	ReadUnlock(string_view name);
private:
	shared_timed_mutex mMutex;
};

class ReadLockGuard
{
public:
	ReadLockGuard(Lock& lock, string_view name) : mLock(lock), mName(name) { mLock.ReadLock(name); }
	~ReadLockGuard() { mLock.ReadUnlock(mName); }
private:
	Lock& mLock;
	string mName;
};

class WriteLockGuard
{
public:
	WriteLockGuard(Lock& lock, string_view name) : mLock(lock), mName(name) { mLock.WriteLock(name); }
	~WriteLockGuard() { mLock.WriteUnlock(mName); }
private:
	Lock& mLock;
	string mName;
};