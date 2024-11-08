using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
	#region Singleton
    static PacketManager _instance = new PacketManager();
	public static PacketManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = new PacketManager();
			return _instance;
		}
	}
	#endregion

	PacketManager()
    {
        Register();
    }

	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
		
	public void Register()
	{
		_makeFunc.Add((ushort)PacketID.S2C_BroadcastEnterGame, MakePacket<S2C_BroadcastEnterGame>);
		_handler.Add((ushort)PacketID.S2C_BroadcastEnterGame, PacketHandler.S2C_BroadcastEnterGameHandler);
		_makeFunc.Add((ushort)PacketID.S2C_BroadcastLeaveGame, MakePacket<S2C_BroadcastLeaveGame>);
		_handler.Add((ushort)PacketID.S2C_BroadcastLeaveGame, PacketHandler.S2C_BroadcastLeaveGameHandler);
		_makeFunc.Add((ushort)PacketID.S2C_PlayerList, MakePacket<S2C_PlayerList>);
		_handler.Add((ushort)PacketID.S2C_PlayerList, PacketHandler.S2C_PlayerListHandler);
		_makeFunc.Add((ushort)PacketID.S2C_BroadcastMove, MakePacket<S2C_BroadcastMove>);
		_handler.Add((ushort)PacketID.S2C_BroadcastMove, PacketHandler.S2C_BroadcastMoveHandler);

	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);
            if(onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlerPacket(session, packet);
        }
	}

	T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
        pkt.Read(buffer);
        return pkt;
	}

	public void HandlerPacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }
}