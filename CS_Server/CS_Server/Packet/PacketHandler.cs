
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

class PacketHandler
{
    public static void C2S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        ClientSession? clientSession = session as ClientSession;
        if (clientSession == null)
        {
            return;
        }

        if (clientSession.Room == null)
        {
            return;
        }

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        C2S_Move? movePacket = packet as C2S_Move;
        ClientSession? clientSession = session as ClientSession;

        if (clientSession == null || movePacket == null)
        {
            return;
        }

        if (clientSession.Room == null)
        {
            return;
        }

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }
}
