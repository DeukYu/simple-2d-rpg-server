
using Google.Protobuf;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using Shared;
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

    private static bool TryParsePacket<T>(PacketSession session, IMessage packet, out ClientSession? clientSession, out T? typedPacket) where T : class, IMessage
    {
        clientSession = session as ClientSession;
        typedPacket = packet as T;

        if (clientSession == null || typedPacket == null)
        {
            Log.Error($"Invalid packet. session: {session.GetType()}, packet: {packet.GetType()}");
            return false;
        }

        return true;
    }

    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        if (!TryParsePacket<C2S_Move>(session, packet, out var clientSession, out var movePacket))
            return;

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

        zone.Push(zone.HandleMove, player, movePacket!);
    }

    public static void C2S_SkillHandler(PacketSession session, IMessage packet)
    {
        if (!TryParsePacket<C2S_Skill>(session, packet, out var clientSession, out var skillPacket))
            return;

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

        zone.Push(zone.HandleSkill, player, skillPacket!);
    }
    public static void C2S_LoginHandler(PacketSession session, IMessage packet)
    {
        if (!TryParsePacket<C2S_Login>(session, packet, out var clientSession, out var loginPacket))
            return;

        int state = clientSession.HandleLogin(loginPacket.UniqueId, out var lobbyPlayers);
        var res = new S2C_Login
        {
            Players = { lobbyPlayers },
            Result = state
        };
    }

    public static void C2S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        if (!TryParsePacket<C2S_CreatePlayer>(session, packet, out var clientSession, out var createPlayerPacket))
            return;

        int state = clientSession.HandleCreatePlayer(createPlayerPacket.Name, out var lobbyPlayer);
        var res = new S2C_CreatePlayer
        {
            Player = lobbyPlayer,
            Result = state
        };
    }

    public static void C2S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        if (!TryParsePacket<C2S_EnterGame>(session, packet, out var clientSession, out var enterGamePacket))
            return;
        Log.Info($"C2S_EnterGameHandler: {enterGamePacket}");
        
        clientSession.HandleEnterGame(enterGamePacket);
    }
}
