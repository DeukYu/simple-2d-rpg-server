using System.Collections.Concurrent;

namespace ServerCore;

public interface IJobQueue
{
    void Push(Action job);
}
public class JobQueue
{
    private ConcurrentQueue<Action> _jobQueue = new ConcurrentQueue<Action>();
    private AtomicFlag _flush = new AtomicFlag();
    private object _lock = new object();

    public void Push(Action job)
    {
        AtomicFlag flush = new AtomicFlag();

        lock (_lock)
        {
            _jobQueue.Enqueue(job);
            if (_flush == false)
            {
                flush.Set();
                _flush.Set();
            }
        }

        if (flush)
            Flush();
    }

    void Flush()
    {
        while (true)
        {
            Action? action = Pop();
            if (action == null)
                return;

            action.Invoke();
        }
    }

    public Action? Pop()
    {
        lock (_lock)
        {
            if (_jobQueue.Count == 0)
            {
                _flush.Release();
                return null;
            }

            return _jobQueue.TryDequeue(out Action? action) ? action : null;
        }
    }
}
