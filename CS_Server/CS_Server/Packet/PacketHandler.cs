
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
        // 핸들러 이름 생성 규칙에 맞춰 이름 설정 (예: 타입 이름 + "Handler")
        string handlerMethodName = $"{packetType.Name}Handler";

        // 현재 클래스(PacketHandler)에서 해당 이름을 가진 메서드를 찾음
        var methodInfo = typeof(PacketHandler).GetMethod(handlerMethodName,
                            BindingFlags.Public | BindingFlags.Static);

        // 핸들러가 없으면 null 반환
        if (methodInfo == null)
            return null;

        // 메서드 정보를 Action<PacketSession, IMessage> 델리게이트로 변환
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

    public static void C2S_ChatHandler(PacketSession session, IMessage packet)
    {
        C2S_Chat? chatPacket = packet as C2S_Chat;
        ClientSession? clientSession = session as ClientSession;

        if(clientSession == null || chatPacket == null)
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
