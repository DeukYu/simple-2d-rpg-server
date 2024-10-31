using ServerCore;

namespace CS_Server;

class GameRoom : IJobQueue
{
    List<ClientSession> _sessions = new List<ClientSession>();
    JobQueue _jobQueue = new JobQueue();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    public void Push(Action job)
    {
        _jobQueue.Push(job);
    }

    public void Flush()
    {
        foreach (ClientSession s in _sessions)
            s.Send(_pendingList);

        Log.Info($"Flushed {_pendingList.Count} items");
        _pendingList.Clear();
    }

    public void Broadcast(ClientSession session, string chat)
    {
        S2C_Chat res = new S2C_Chat();
        res.playerId = session.SessionId;
        res.chat = chat + $"I am {res.playerId}";
        ArraySegment<byte> segment = res.Write();

        _pendingList.Add(segment);
    }

    public void Enter(ClientSession session)
    {
        _sessions.Add(session);
        session.Room = this;
    }

    public void Leave(ClientSession session)
    {
        _sessions.Remove(session);
    }
}
