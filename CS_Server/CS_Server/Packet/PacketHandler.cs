using ServerCore;

namespace CS_Server;

class PacketHandler
{
    public static void C2S_ChatHandler(PacketSession session, IPacket packet)
    {
        C2S_Chat req = packet as C2S_Chat;

        ClientSession clientSession = session as ClientSession;

        if(clientSession.Room == null)
        {
            Log.Error("C2S_ChatHandler no room");
            return;
        }

        clientSession.Room.Broadcast(clientSession, req.chat);
    }
}
