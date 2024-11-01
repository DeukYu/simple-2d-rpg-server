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

    public void Broadcast(ArraySegment<byte> segment)
    {
        _pendingList.Add(segment);
    }

    public void Enter(ClientSession session)
    {
        _sessions.Add(session);
        session.Room = this;

        // 모든 플레이어 목록 전송
        S2C_PlayerList res = new S2C_PlayerList();
        foreach (ClientSession s in _sessions)
        {
            res.players.Add(new S2C_PlayerList.Player()
            {
                isSelf = (s == session),
                playerId = s.SessionId,
                posX = s.PosX,
                posY = s.PosY,
                posZ = s.PosZ
            });
        }
        session.Send(res.Write());

        // 새로운 플레이어 입장을 모두에게 알림
        S2C_BroadcastEnterGame enter = new S2C_BroadcastEnterGame();

        enter.playerId = session.SessionId;
        enter.posX = session.PosX;
        enter.posY = session.PosY;
        enter.posZ = session.PosZ;
        Broadcast(enter.Write());
    }

    public void Leave(ClientSession session)
    {
        // 플레이어 제거
        _sessions.Remove(session);

        // 모두에게 알림
        S2C_BroadcastLeaveGame leave = new S2C_BroadcastLeaveGame();
        leave.playerId = session.SessionId;
        Broadcast(leave.Write());
    }

    public void Move(ClientSession session, C2S_Move packet)
    {
        session.PosX = packet.posX;
        session.PosY = packet.posY;
        session.PosZ = packet.posZ;

        S2C_BroadcastMove move = new S2C_BroadcastMove();
        move.playerId = session.SessionId;
        move.posX = session.PosX;
        move.posY = session.PosY;
        move.posZ = session.PosZ;

        // 모두에게 알림
        Broadcast(move.Write());
    }
}
