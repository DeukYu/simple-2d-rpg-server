using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class JobSerializer
{
    private ConcurrentQueue<IJob> _jobQueue = new ConcurrentQueue<IJob>();
    private AtomicFlag _flush = new AtomicFlag();

    public void Push(Delegate action, params object[] parameters)
    {
        Push(new Job(action, parameters));
    }

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
