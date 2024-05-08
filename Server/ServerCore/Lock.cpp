#include "pch.h"
#include "Lock.h"
#include "CoreTLS.h"
#include "CoreMacro.h"

void Lock::WriteLock(string_view name)
{
#if _DEBUG
	//GDeadLockProfiler->PushLock(name);
#endif
	if (m_mutex.try_lock_for(chrono::milliseconds(100)) == false)
		ASSERT_CRASH("WRITELOCK_TIMEOUT");
}

void Lock::WriteUnlock(string_view name)
{
#if _DEBUG
	//GDeadLockProfiler->PopLock(name);
#endif
	m_mutex.unlock();
}

void Lock::ReadLock(string_view name)
{
#if _DEBUG
	//GDeadLockProfiler->PushLock(name);
#endif
	if (m_mutex.try_lock_shared_for(chrono::milliseconds(100)) == false)
		ASSERT_CRASH("READLOCK_TIMEOUT");
}

void Lock::ReadUnlock(string_view name)
{
#if _DEBUG
	//GDeadLockProfiler->PopLock(name);
#endif
	m_mutex.unlock_shared();
}