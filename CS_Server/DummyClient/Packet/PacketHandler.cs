using ServerCore;

namespace DummyClient;

class PacketHandler
{
    public static void S2C_BroadcastEnterGameHandler(PacketSession session, IPacket packet) 
    {
        S2C_BroadcastEnterGame? res = packet as S2C_BroadcastEnterGame;
        if (res == null)
        {
            return;
        }

        ServerSession? serverSession = session as ServerSession;
        if (serverSession == null)
        {
            return;
        }
    }

    public static void S2C_BroadcastLeaveGameHandler(PacketSession session, IPacket packet) { }

    public static void S2C_PlayerListHandler(PacketSession session, IPacket packet) { }

    public static void S2C_BroadcastMoveHandler(PacketSession session, IPacket packet) { }
}
