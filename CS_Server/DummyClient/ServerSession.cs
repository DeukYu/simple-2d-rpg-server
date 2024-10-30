using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient;

class ServerSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        C2S_PlayerInfoReq packet = new C2S_PlayerInfoReq() { playerId = 1001, name = "ABCD" };

        var skill = new C2S_PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f };
        skill.attributes.Add(new C2S_PlayerInfoReq.Skill.Attribute() { att = 77 });
        packet.skills.Add(skill);

        packet.skills.Add(new C2S_PlayerInfoReq.Skill() { id = 201, level = 2, duration = 4.0f });
        packet.skills.Add(new C2S_PlayerInfoReq.Skill() { id = 301, level = 3, duration = 5.0f });
        packet.skills.Add(new C2S_PlayerInfoReq.Skill() { id = 401, level = 4, duration = 6.0f });

        var s = packet.Write();
        if (s != null)
            Send(s);
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