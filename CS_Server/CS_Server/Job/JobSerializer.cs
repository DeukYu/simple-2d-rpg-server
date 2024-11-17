using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class JobSerializer
{
    private ConcurrentQueue<IJob> _jobQueue = new ConcurrentQueue<IJob>();
    private AtomicFlag _flush = new AtomicFlag();

    public void Push(Action action) { Push(new Job(action)); }
    public void Push<T1>(Action<T1> action, T1 t1) { Push(new Job<T1>(action, t1)); }
    public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2) { Push(new Job<T1, T2>(action, t1, t2)); }
    public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { Push(new Job<T1, T2, T3>(action, t1, t2, t3)); }

    public void Push(IJob job)
    {
        AtomicFlag flush = new AtomicFlag();

        _jobQueue.Enqueue(job);
        if (_flush == false)
        {
            flush.Set();
            _flush.Set();
        }

        if (flush)
            Flush();
    }

    void Flush()
    {
        while (true)
        {
            var job = Pop();
            if (job == null)
                return;

            job.Execute();
        }
    }

    public IJob? Pop()
    {
        if (_jobQueue.Count == 0)
        {
            _flush.Release();
            return null;
        }

        return _jobQueue.TryDequeue(out IJob? action) ? action : null;
    }
}
