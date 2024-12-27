using Google.Protobuf.Reflection;
using Google.Protobuf;
using ServerCore;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using DummyClient.Packet;

class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
    private Dictionary<Type, ushort> _typeToMsgId = new Dictionary<Type, ushort>();

    public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        // 현재 어셈블리에서 IMessage를 구현한 비추상 타입 가져오기
        var packetTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(IMessage).IsAssignableFrom(x) && !x.IsAbstract);

        foreach (var packetType in packetTypes)
        {
            // Descriptor를 가져오고, null 체크
            var descriptor = GetMessageDescriptor(packetType);
            if (descriptor == null)
                continue;

            // 메시지 이름으로 메시지 ID 계산
            ushort messageId = ComputeMessageId(descriptor.Name);
            if (_onRecv.ContainsKey(messageId))
            {
                Log.Error($"Already registered message: {messageId}");
                continue;
            }

            // MakePacket<T>를 호출하는 델리게이트 생성
            var makePacketAction = CreateMakePacketAction(packetType);

            // 델리게이트 및 핸들러 등록
            _onRecv.Add(messageId, makePacketAction);
            RegisterHandler(messageId, packetType);
            _typeToMsgId.Add(packetType, messageId);
        }
    }

    public ushort GetMessageId(Type packetType)
    {
        _typeToMsgId.TryGetValue(packetType, out ushort id);
        return id;
    }

    // 메시지 ID 등록 메서드
    private void RegisterHandler(ushort messageId, Type packetType)
    {
        var handler = PacketHandler.GetHandler(packetType);
        if (handler != null)
        {
            _handler[messageId] = handler;
        }
    }

    // Descriptor를 가져오는 메서드
    private MessageDescriptor GetMessageDescriptor(Type packetType)
    {
        var descriptor = packetType.GetProperty("Descriptor", BindingFlags.Public | BindingFlags.Static)?
                                    .GetValue(null) as MessageDescriptor;
        if (descriptor == null)
        {
            Log.Error($"Descriptor not found for packet type: {packetType.Name}");
            return null;
        }

        return descriptor;
    }

    // MakePacket 호출을 위한 델리게이트 생성 메서드
    private Action<PacketSession, ArraySegment<byte>, ushort> CreateMakePacketAction(Type packetType)
    {
        return (session, buffer, id) =>
        {
            MethodInfo method = typeof(PacketManager).GetMethod(nameof(MakePacket), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericMethod = method.MakeGenericMethod(packetType);
            genericMethod.Invoke(this, new object[] { session, buffer, id });
        };
    }

    private ushort ComputeMessageId(string messageName)
    {
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(messageName));
        return BitConverter.ToUInt16(hash, 0);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer, id);
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        T pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

        if (CustomHandler != null)
        {
            CustomHandler.Invoke(session, pkt, id);
        }
        else
        {
            if (_handler.TryGetValue(id, out var action))
                action.Invoke(session, pkt);
        }
    }

    public Action<PacketSession, IMessage>? GetPacketHandler(ushort id)
    {
        if (_handler.TryGetValue(id, out var action))
            return action;
        return null;
    }
}