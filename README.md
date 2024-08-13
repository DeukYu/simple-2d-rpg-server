# simple-3d-mmorpg-server
 
## ServerCore
### 동기화   
shared_timed_mutex 로 ReadLock과 WriteLock을 구현하여 LockGuard를 만들었습니다.
```cpp
class Lock
{
public:
    void WriteLock(string_view name);
    void WriteUnlock(string_view name);
    void ReadLock(string_view name);
    void ReadUnlock(string_view name);
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
```

