using Google.Protobuf;
using Google.Protobuf.Enum;
using ServerCore;
using System.Net;
using System.Text;

namespace CS_Server;

public class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public Player GamePlayer { get; set; }
    public Zone Zone { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

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

        GamePlayer = PlayerManager.Instance.Add(this);
        {
            GamePlayer._playerInfo.Name = $"Player_{GamePlayer._playerInfo.PlayerId}";
            GamePlayer._playerInfo.PosX = 0;
            GamePlayer._playerInfo.PosY = 0;
        }

        ZoneManager.Instance.FindZone(1).EnterZone(GamePlayer);
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
        ZoneManager.Instance.FindZone(1).LeaveZone(GamePlayer._playerInfo.PlayerId);

        Log.Info($"OnDisConnected: {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
        Log.Info($"Transferred bytes: {numOfBytes}");
    }
}