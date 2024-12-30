using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class JobSerializer
{
    private readonly JobTimer _timer = new JobTimer();
    private readonly ConcurrentQueue<JobBase> _jobQueue = new ConcurrentQueue<JobBase>();
    private readonly AtomicFlag _flushInProgress = new AtomicFlag();

    // 일정 시간 후에 작업을 스케줄링
    public JobBase ScheduleJobAfterDelay(int delayTicks, Delegate action, params object[] parameters)
    {
        if(action == null)
            throw new ArgumentNullException(nameof(action));

        var job = new Job(action, parameters);
        _timer.Push(job, delayTicks);
        return job;
    }

    // 
    public void ScheduleJob(Delegate action, params object[] parameters)
    {
        var job = new Job(action, parameters);
        _jobQueue.Enqueue(job);
    }

    public void ProcessJobs()
    {
        _timer.Flush();

        while (true)
        {
            var job = DequeueJob();
            if (job == null)
                return;

            job.Execute();
        }
    }

    // 작업을 큐에서 제거하고 반환
    public JobBase? DequeueJob()
    {
        if (_jobQueue.Count == 0)
        {
            _flushInProgress.Release();
            return null;
        }

        return _jobQueue.TryDequeue(out JobBase? job) ? job : null;
    }
}
