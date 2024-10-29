
using ServerCore;
using System.Net;
using System.Text;

namespace CS_Server;

class Knight
{
    public int hp;
    public int attack;
}

class GameSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        Knight knight = new Knight() { hp = 100, attack = 10 };

        var openSegment = SendBufferHelper.Open(4096);
        var buffer1 = BitConverter.GetBytes(knight.hp);
        var buffer2 = BitConverter.GetBytes(knight.attack);
        Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
        Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
        var sendBuff = SendBufferHelper.Close(buffer1.Length + buffer2.Length);

        Send(sendBuff);
        Thread.Sleep(1000);
        Disconnect();
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
        Log.Info($"[From Client] {recvData}");
        return buffer.Count;
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