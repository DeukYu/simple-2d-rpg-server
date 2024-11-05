
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
        zone.Push(() => zone.Leave(clientSession));
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
        room.Push(() => room.Move(clientSession, movePacket));
    }

    public static void C2S_ChatHandler(PacketSession session, IMessage packet)
    {
        C2S_Chat? chatPacket = packet as C2S_Chat;
        ClientSession? clientSession = session as ClientSession;
        if (clientSession == null || chatPacket == null)
        {
            return;
        }

        S2C_BroadcastChat chat = new S2C_BroadcastChat();
        chat.PlayerId = clientSession.SessionId;
        chat.Chat = chatPacket.Chat;

        Log.Info($"Player({clientSession.SessionId}): {chatPacket.Chat}");

        ushort size = (ushort)chat.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];
        Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
        ushort protocolId = PacketManager.Instance.GetMessageId(chat.GetType());
        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
        Array.Copy(chat.ToByteArray(), 0, sendBuffer, 4, size);
        var buff = new ArraySegment<byte>(sendBuffer);

        clientSession.Send(buff);
    }
}
