using Google.Protobuf;
using Google.Protobuf.Enum;
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
        // TODO : 현재는 정보들을 간단하게 받기 위해 임시로 만들어 놓은 것이므로 추후에 수정해야 한다.
        GamePlayer = ObjectManager.Instance.Add<Player>();
        {
            GamePlayer.Info.Name = $"Player_{GamePlayer.Info.ObjectId}";
            GamePlayer.Info.PosInfo.State = CreatureState.Idle;
            GamePlayer.Info.PosInfo.MoveDir = MoveDir.Down;
            GamePlayer.Info.PosInfo.PosX = 0;
            GamePlayer.Info.PosInfo.PosY = 0;

            if (DataManager.StatDict.TryGetValue(1, out var stat))
            {
                GamePlayer.StatInfo.Level = 1;
                GamePlayer.StatInfo.Hp = stat.MaxHp;
                GamePlayer.StatInfo.MaxHp = stat.MaxHp;
                GamePlayer.StatInfo.Mp = stat.MaxMp;
                GamePlayer.StatInfo.MaxHp = stat.MaxHp;
                GamePlayer.StatInfo.Attack = stat.Attack;
                GamePlayer.StatInfo.Exp = 0;
                GamePlayer.StatInfo.TotalExp = stat.TotalExp;
            }

            GamePlayer.Session = this;
        }

        if (GamePlayer == null)
        {
            Log.Error("OnConnected: GamePlayer is null.");
            return;
        }

        var zone = ZoneManager.Instance.FindZone(1);
        if (zone == null)
        {
            Log.Error("OnConnected: zone is null");
            return;
        }

        zone.Push(zone.EnterZone, GamePlayer);
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
        var zone = ZoneManager.Instance.FindZone(1);
        if (zone == null)
        {
            Log.Error("OnDisConnected: zone is null");
            return;
        }

        zone.Push(zone.LeaveZone, GamePlayer);

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