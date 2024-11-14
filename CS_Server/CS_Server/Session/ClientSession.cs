using Google.Protobuf;
using ServerCore;
using System.Net;

namespace CS_Server;

public class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public Player? GamePlayer { get; set; }

    public void Send(IMessage packet)
    {
        ushort size = (ushort)packet.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];

        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));

        ushort protocolId = PacketManager.Instance.GetMessageId(packet.GetType());
        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
        
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
        Send(new ArraySegment<byte>(sendBuffer));
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        // TODO : 연결 되었을 때, 임시적으로 바로 zone에 입장시킨다.
        GamePlayer = ObjectManager.Instance.Add<Player>();

        if (GamePlayer == null)
        {
            Log.Error("OnConnected: GamePlayer is null.");
            return;
        }

        var zone = ZoneManager.Instance.FindZone(1);
        if(zone == null)
        {
            Log.Error("OnConnected: zone is null");
            return;
        }

        zone.EnterZone(GamePlayer);
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
        ZoneManager.Instance.FindZone(1).LeaveZone(GamePlayer);

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