using CS_Server;

namespace ServerCore;

public class JobTimer
{
    private PriorityQueue<JobBase, long> _pq = new PriorityQueue<JobBase, long>();
    private readonly object _lock = new object();

    public void Push(JobBase job, int delayTicks = 0)
    {
        var execTick = DateTime.UtcNow.Ticks + TimeSpan.FromMilliseconds(delayTicks).Ticks;

        lock (_lock)
        {
            _pq.Enqueue(job, execTick);
        }
    }

    public void Flush()
    {
        while (true)
        {
            var now = DateTime.UtcNow.Ticks;

            JobBase? job = null!;

            lock (_lock)
            {
                if (_pq.Count == 0)
                    break;

                if (!_pq.TryPeek(out job, out var execTic) || execTic > now)
                    break;

                _pq.TryDequeue(out job, out _);
            }
            job?.Execute();
        }
    }
}
