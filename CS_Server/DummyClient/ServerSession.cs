using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient;

public abstract class Packet
{
    public ushort size;
    public ushort packetId;

    public abstract ArraySegment<byte> Write();
    public abstract void Read(ArraySegment<byte> segment);
}

class PlayerInfoReq : Packet
{
    public long playerId;
    public string name;

    public struct SkillInfo
    {
        public int id;
        public short level;
        public float duration;

        public bool Write(Span<byte> s, ref ushort count)
        {
            bool success = true;

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
            count += sizeof(int);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
            count += sizeof(short);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
            count += sizeof(float);

            return true;
        }

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {
            id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);
            level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
            count += sizeof(short);
            duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);
        }
    }

    public List<SkillInfo> skills = new List<SkillInfo>();

    public PlayerInfoReq()
    {
        packetId = (ushort)PacketID.PlayerInfoReq;
    }
    public override void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> span = segment.AsSpan();
        //var size = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        //ushort packetId = BitConverter.ToUInt16(s.Array, s.Offset + count);
        count += sizeof(ushort);
        playerId = BitConverter.ToInt64(span.Slice(count, span.Length - count));
        count += sizeof(long);

        // string
        ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
        count += sizeof(ushort);
        name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
        count += nameLen;

        // skill list
        skills.Clear();
        ushort skillLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
        count += sizeof(ushort);
        for (int i=0;i< skillLen; ++i)
        {
            SkillInfo skill = new SkillInfo();
            skill.Read(span, ref count);
            skills.Add(skill);
        }
    }
    public override ArraySegment<byte> Write()
    {
        var s = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        var span = s.AsSpan();

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, span.Length - count), packetId);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, span.Length - count), playerId);
        count += sizeof(long);


        // string
        //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(name);
        //success &= BitConverter.TryWriteBytes(s.Slice(count, span.Length - count), nameLen);
        //count += sizeof(ushort);
        //Array.Copy(Encoding.Unicode.GetBytes(name), 0, s.Array, count, nameLen);
        //count += nameLen;

        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(name, 0, name.Length, s.Array, s.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, span.Length - count), nameLen);
        count += sizeof(ushort);
        count += nameLen;

        // skill list
        success &= BitConverter.TryWriteBytes(s.Slice(count, span.Length - count), (ushort)skills.Count);
        count += sizeof(ushort);
        foreach (var skill in skills)
        {
            success &= skill.Write(s, ref count);
        }

        success &= BitConverter.TryWriteBytes(s, count);

        if (success == false)
            return null;

        return SendBufferHelper.Close(count);
    }
}

//class PlayerInfoRes : Packet
//{
//    public int hp;
//    public int attack;
//}

public enum PacketID
{
    PlayerInfoReq = 1,
    PlayerInfoRes = 2,
}

class ServerSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");

        PlayerInfoReq req = new PlayerInfoReq() { playerId = 1001, name = "ABCD" };
        req.skills.Add(new PlayerInfoReq.SkillInfo() { id = 101, level = 1, duration = 3.0f });
        req.skills.Add(new PlayerInfoReq.SkillInfo() { id = 102, level = 1, duration = 3.0f });
        req.skills.Add(new PlayerInfoReq.SkillInfo() { id = 103, level = 1, duration = 3.0f });
        var s = req.Write();
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