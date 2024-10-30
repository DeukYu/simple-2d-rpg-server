using ServerCore;
using System.Net;
using System.Text;

namespace CS_Server;

class ClientSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        Thread.Sleep(5000);
        Disconnect();
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
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