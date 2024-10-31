namespace ServerCore;

public class JobTimer
{
    PriorityQueue<Action, int> _pq = new PriorityQueue<Action, int>();
    object _lock = new object();

    public static JobTimer Instance { get; } = new JobTimer();

    public void Push(Action action, int tickAfter = 0)
    {
        int execTick = System.Environment.TickCount + tickAfter;

        lock (_lock)
        {
            _pq.Enqueue(action, execTick);
        }
    }

    public void Flush()
    {
        while (true)
        {
            int now = System.Environment.TickCount;

            Action? job = null!;
            lock (_lock)
            {
                if (_pq.Count == 0)
                    break;

                if (!_pq.TryPeek(out job, out int execTic) || execTic > now)
                    break;

                _pq.TryDequeue(out job, out _);
            }
            job?.Invoke();
        }
    }
}
