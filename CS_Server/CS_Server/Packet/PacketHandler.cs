
using Google.Protobuf;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Reflection;

namespace CS_Server;

class PacketHandler
{
    public static Action<PacketSession, IMessage>? GetHandler(Type packetType)
    {
        string handlerMethodName = $"{packetType.Name}Handler";

        var methodInfo = typeof(PacketHandler).GetMethod(handlerMethodName, BindingFlags.Public | BindingFlags.Static);
        if (methodInfo == null)
            return null;

        return (Action<PacketSession, IMessage>)Delegate.CreateDelegate(
            typeof(Action<PacketSession, IMessage>), methodInfo);
    }

    public static void C2S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        ClientSession? clientSession = session as ClientSession;
        if (clientSession == null)
        {
            return;
        }

        if (clientSession.Zone == null)
        {
            return;
        }

        Zone zone = clientSession.Zone;
    }

    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        C2S_Move? movePacket = packet as C2S_Move;
        ClientSession? clientSession = session as ClientSession;

        if (clientSession == null || movePacket == null)
        {
            return;
        }

        if (clientSession.Zone == null)
        {
            return;
        }

        Zone room = clientSession.Zone;
    }
}
