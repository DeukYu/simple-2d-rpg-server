using ServerCore;

namespace DummyClient;

class PacketHandler
{
    public static void S2C_ChatHandler(PacketSession session, IPacket packet)
    {
        S2C_Chat pkt = packet as S2C_Chat;
        ServerSession serverSession = session as ServerSession;

        if (pkt.playerId == 1)
            Log.Info($"[PacketHandler] S2C_ChatHandler: {pkt.chat}");

    }
}
