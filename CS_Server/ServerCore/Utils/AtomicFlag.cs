namespace ServerCore;

public sealed class AtomicFlag
{
    private volatile int _flag = 0;

    public static implicit operator bool(AtomicFlag target)
    {
        // true = 1 이고 false = 0 이기 때문에 1이면 true를 반환
        return target._flag == 1;
    }
    public bool Set()
    {
        // 현재 false 일 경우, true로 바꾸고 false를 반환
        return Interlocked.CompareExchange(ref _flag, 1, 0) == 0;
    }

    public void Release()
    {
        // false로 셋팅
        Interlocked.Exchange(ref _flag, 0);
    }
}
