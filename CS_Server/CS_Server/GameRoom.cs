using Google.Protobuf;
using Google.Protobuf.Common;
using Google.Protobuf.Protocol;
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
            res.Players.Add(new TPlayer
            {
                IsSelf = (s == session),
                PlayerId = s.SessionId,
                PosX = s.PosX,
                PosY = s.PosY,
                PosZ = s.PosZ
            });
        }

        session.Send(res.ToByteArray());

        // 새로운 플레이어 입장을 모두에게 알림
        S2C_BroadcastEnterGame enter = new S2C_BroadcastEnterGame();

        enter.PlayerId = session.SessionId;
        enter.PosX = session.PosX;
        enter.PosY = session.PosY;
        enter.PosZ = session.PosZ;
        Broadcast(enter.ToByteArray());
    }

    public void Leave(ClientSession session)
    {
        // 플레이어 제거
        _sessions.Remove(session);

        // 모두에게 알림
        S2C_BroadcastLeaveGame leave = new S2C_BroadcastLeaveGame();
        leave.PlayerId = session.SessionId;

        Broadcast(leave.ToByteArray());
    }

    public void Move(ClientSession session, C2S_Move packet)
    {
        session.PosX = packet.PosX;
        session.PosY = packet.PosY;
        session.PosZ = packet.PosZ;

        S2C_BroadcastMove move = new S2C_BroadcastMove();
        move.PlayerId = session.SessionId;
        move.PosX = session.PosX;
        move.PosY = session.PosY;
        move.PosZ = session.PosZ;

        // 모두에게 알림
        Broadcast(move.ToByteArray());
    }
}
