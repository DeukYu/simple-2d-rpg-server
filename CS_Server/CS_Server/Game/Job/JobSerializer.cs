using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class JobSerializer
{
    private JobTimer _timer = new JobTimer();
    private ConcurrentQueue<IJob> _jobQueue = new ConcurrentQueue<IJob>();
    private AtomicFlag _flush = new AtomicFlag();

    public void PushAfter(int tickAfter, Delegate action, params object[] parameters)
    {
        PushAfter(tickAfter, new Job(action, parameters));
    }

    public void PushAfter(int tickAfter, IJob job)
    {
        _timer.Push(job, tickAfter);
    }

    public void Push(Delegate action, params object[] parameters)
    {
        Push(new Job(action, parameters));
    }

    public void Push(IJob job)
    {
        _jobQueue.Enqueue(job);
    }

    public void Flush()
    {
        _timer.Flush();

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
