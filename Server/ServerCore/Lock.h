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
	shared_timed_mutex m_mutex;
};

class ReadLockGuard
{
public:
	ReadLockGuard(Lock& lock, string_view name) : _lock(lock), _name(name) { _lock.ReadLock(name); }
	~ReadLockGuard() { _lock.ReadUnlock(_name); }
private:
	Lock& _lock;
	string _name;
};

class WriteLockGuard
{
public:
	WriteLockGuard(Lock& lock, string_view name) : _lock(lock), _name(name) { _lock.WriteLock(name); }
	~WriteLockGuard() { _lock.WriteUnlock(_name); }
private:
	Lock& _lock;
	string _name;
};