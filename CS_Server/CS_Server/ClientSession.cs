using ServerCore;
using System.Net;
using System.Text;

namespace CS_Server;

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
        for (int i = 0; i < skillLen; ++i)
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

class ClientSession : PacketSession
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
        var count = 0;
        var size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        switch((PacketID)packetId)
        {
            case PacketID.PlayerInfoReq:
                {
                    PlayerInfoReq p = new PlayerInfoReq();
                    p.Read(buffer);
                    Log.Info($"PlayerInfoReq: {p.playerId} {p.name}");

                    foreach(var skill in p.skills)
                    {
                        Log.Info($"Skill: {skill.id} {skill.level} {skill.duration}");
                    }
                }
                break;

        }
        Log.Info($"PacketId: {packetId}, Size: {size}");
    }

    public override void OnSend(int numOfBytes)
    {
        Log.Info($"Transferred bytes: {numOfBytes}");
    }
}