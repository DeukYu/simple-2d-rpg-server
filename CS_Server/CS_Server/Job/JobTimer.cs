using CS_Server;

namespace ServerCore;

public class JobTimer
{
    PriorityQueue<IJob, int> _pq = new PriorityQueue<IJob, int>();
    object _lock = new object();

    public void Push(IJob job, int tickAfter = 0)
    {
        int execTick = System.Environment.TickCount + tickAfter;

        lock (_lock)
        {
            _pq.Enqueue(job, execTick);
        }
    }

    public void Flush()
    {
        while (true)
        {
            int now = System.Environment.TickCount;

            IJob? job = null!;
            lock (_lock)
            {
                if (_pq.Count == 0)
                    break;

                if (!_pq.TryPeek(out job, out int execTic) || execTic > now)
                    break;

                _pq.TryDequeue(out job, out _);
            }
            job?.Execute();
        }
    }
}
