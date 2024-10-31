using ServerCore;
using System.Net;
using System.Text;

namespace CS_Server;

class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public GameRoom Room { get; set; }
    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        Program.Room.Push(() => Program.Room.Enter(this));
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
        SessionManager.Instance.Remove(this);
        if (Room != null)
        {
            GameRoom room = Room;
            room.Push(() => room.Leave(this));
            Room = null;
        }
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