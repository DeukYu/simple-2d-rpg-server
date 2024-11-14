
using Google.Protobuf;
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

    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        C2S_Move? movePacket = packet as C2S_Move;
        ClientSession? clientSession = session as ClientSession;
        if (clientSession == null || movePacket == null)
        {
            Log.Error("C2S_MoveHandler: Invalid packet.");
            return;
        }

        var player = clientSession.GamePlayer;
        if (player == null)
        {
            Log.Error("C2S_MoveHandler: GamePlayer is null.");
            return;
        }

        var zone = player._zone;
        if (zone == null)
        {
            Log.Error("C2S_MoveHandler: Zone is null.");
            return;
        }

        zone.HandleMove(player, movePacket);
    }

    public static void C2S_SkillHandler(PacketSession session, IMessage packet)
    {
        C2S_Skill? skillPacket = packet as C2S_Skill;
        ClientSession? clientSession = session as ClientSession;
        if (clientSession == null || skillPacket == null)
        {
            Log.Error("C2S_SkillHandler: Invalid packet.");
            return;
        }

        var player = clientSession.GamePlayer;
        if (player == null)
        {
            Log.Error("C2S_SkillHandler: GamePlayer is null");
            return;
        }
        var zone = player._zone;
        if (zone == null)
        {
            Log.Error("C2S_SkillHandler: Zone is null");
            return;
        }

        zone.HandleSkill(player, skillPacket);
    }
}
