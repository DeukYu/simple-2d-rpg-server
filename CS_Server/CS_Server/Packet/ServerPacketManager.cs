using CS_Server;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.Reflection;
using ServerCore;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class)]
public class MsgIdAttribute : Attribute
{

}

class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public void Register()
    {
        _onRecv.Add((ushort)MsgId.C2SLeavegame, MakePacket<C2S_LeaveGame>);
        _handler.Add((ushort)MsgId.C2SLeavegame, PacketHandler.C2S_LeaveGameHandler);
        _onRecv.Add((ushort)MsgId.C2SMove, MakePacket<C2S_Move>);
        _handler.Add((ushort)MsgId.C2SMove, PacketHandler.C2S_MoveHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (_onRecv.TryGetValue(id, out Action<PacketSession, ArraySegment<byte>, ushort>? action))
            action.Invoke(session, buffer, id);
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        T pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
        if (_handler.TryGetValue(id, out Action<PacketSession, IMessage>? action))
            action.Invoke(session, pkt);
    }

    public Action<PacketSession, IMessage>? GetPacketHandler(ushort id)
    {
        if (_handler.TryGetValue(id, out Action<PacketSession, IMessage>? action))
        {
            return action;
        }
        return null;
    }
}