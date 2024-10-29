
using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient;

class Packet
{
    public ushort size;
    public ushort packetId;
}

class GameSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        for (int i = 0; i < 5; i++)
        {
            Packet packet = new Packet() { size = 4, packetId = 10 };

            var openSegment = SendBufferHelper.Open(4096);
            var buffer1 = BitConverter.GetBytes(packet.size);
            var buffer2 = BitConverter.GetBytes(packet.packetId);
            Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
            var sendBuff = SendBufferHelper.Close(packet.size);
            Send(sendBuff);
        }
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
        Log.Info($"OnDisConnected: {endPoint}");
    }


    public override int OnRecv(ArraySegment<byte> buffer)
    {
        if (buffer.Array == null)
        {
            Log.Error("Buffer array is null");
            return 0;
        }

        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Log.Info($"[From Server] {recvData}");
        return buffer.Count;
    }

    public override void OnSend(int numOfBytes)
    {
        Log.Info($"Transferred bytes: {numOfBytes}");
    }
}
class Program
{
    static void Main(string[] args)
    {
        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return new GameSession(); });

        while (true)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Thread.Sleep(100);
        }
    }
}