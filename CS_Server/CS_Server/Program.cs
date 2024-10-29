using ServerCore;
using System.Net;
using System.Text;

namespace CS_Server;

class Packet
{
    public ushort size;
    public ushort packetId;
}

class GameSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        //Packet packet = new Packet() { size = 4, packetId = 10 };

        //var openSegment = SendBufferHelper.Open(4096);
        //var buffer1 = BitConverter.GetBytes(packet.size);
        //var buffer2 = BitConverter.GetBytes(packet.packetId);
        //Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
        //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
        //var sendBuff = SendBufferHelper.Close(buffer1.Length + buffer2.Length);

        //Send(sendBuff);
        Thread.Sleep(5000);
        Disconnect();
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
        Log.Info($"OnDisConnected: {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
        Log.Info($"PacketId: {packetId}, Size: {size}");
    }

    public override void OnSend(int numOfBytes)
    {
        Log.Info($"Transferred bytes: {numOfBytes}");
    }
}
class Program
{
    static Listener _listener = new Listener();
    static void Main(string[] args)
    {
        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Initialize(endPoint, () => { return new GameSession(); });

        while (true)
        {
            ;
        }

    }
}