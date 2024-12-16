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
            _reserveQueue.Add(new ArraySegment<byte>(sendBuffer, 0, sendBuffer.Length));
        }
    }

    public void FlushSend()
    {
        List<ArraySegment<byte>> sendList = null;
        lock (_lock)
        {
            if (_reserveQueue.Count == 0)
                return;

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

        Log.Info($"OnDisConnected: {endPoint}");
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