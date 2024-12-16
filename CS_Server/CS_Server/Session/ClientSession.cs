using Google.Protobuf;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Net;

namespace CS_Server;

public partial class ClientSession : PacketSession
{
    public ServerState ServerState { get; private set; } = ServerState.Login;
    public Player? GamePlayer { get; set; }
    public int SessionId { get; set; }

    object _lock = new object();
    List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();

    // 패킷 모아보내기
    int _reservedSendBytes = 0;
    long _lastSendTick = 0;

    long _pingpongTick = 0;
    public void Ping()
    {
        if (_pingpongTick > 0)
        {
            var delta = System.Environment.TickCount64 - _pingpongTick;
            if (delta > 30 * 1000)
            {
                Log.Info("Disconnected by PingCheck");
                Disconnect();
                return;
            }
        }

        S2C_Ping pingPacket = new S2C_Ping();
        Send(pingPacket);

        GameLogic.Instance.ScheduleJobAfterDelay(5000, Ping);
    }

    public void HandlePing()
    {
        _pingpongTick = System.Environment.TickCount64;
    }

    // 패킷 예약
    public void Send(IMessage packet)
    {
        ushort size = (ushort)packet.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];

        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));

        ushort protocolId = PacketManager.Instance.GetMessageId(packet.GetType());
        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));

        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

        lock (_lock)
        {
            _reserveQueue.Add(sendBuffer);
            _reservedSendBytes += sendBuffer.Length;
        }
    }

    public void FlushSend()
    {
        List<ArraySegment<byte>> sendList = null;
        lock (_lock)
        {
            if (_reserveQueue.Count == 0)
                return;

            // 0.1초가 지났거나, 일정 패킷이 모였을 때,
            long delta = System.Environment.TickCount64 - _lastSendTick;
            if(delta < 100 && _reservedSendBytes < 10000)
                return;

            // 패킷 모아 보내기
            _reservedSendBytes = 0;
            _lastSendTick = System.Environment.TickCount64;

            sendList = _reserveQueue;
            _reserveQueue = new List<ArraySegment<byte>>();
        }

        Send(sendList);
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        // 연결 성공 패킷 전송
        S2C_Connected connectedPacket = new S2C_Connected { Result = (int)ErrorType.Success };
        Send(connectedPacket);

        GameLogic.Instance.ScheduleJobAfterDelay(5000, Ping);
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
        GameLogic.Instance.ScheduleJob(() =>
        {
            if (GamePlayer == null)
                return;

            Zone zone = GameLogic.Instance.FindZone(1);
            if (zone == null)
            {
                Log.Error("OnConnected: zone is null");
                return;
            }
            zone.ScheduleJob(zone.LeaveZone, GamePlayer);
        });

        SessionManager.Instance.Remove(this);
    }
    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }
    public override void OnSend(int numOfBytes)
    {
        //Log.Info($"Transferred bytes: {numOfBytes}");
    }
}