using DummyClient.Session;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using ServerCore;
using System.Diagnostics;
using System.Reflection;

namespace DummyClient.Packet;

partial class PacketHandler
{
    public static Action<PacketSession, IMessage> GetHandler(Type packetType)
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

    public static void S2C_ItemListHandler(PacketSession session, IMessage packet)
    {
        S2C_ItemList itemListPacket = packet as S2C_ItemList;

    }

    public static void S2C_AddItemHandler(PacketSession session, IMessage packet)
    {
        S2C_AddItem addItemPacket = (S2C_AddItem)packet;

    }

    public static void S2C_EquipItemHandler(PacketSession session, IMessage packet)
    {
        S2C_EquipItem equipItemPacket = (S2C_EquipItem)packet;

        
    }

    public static void S2C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S2C_EnterGame enterPacket = packet as S2C_EnterGame;
        
    }

    public static void S2C_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S2C_LeaveGame leavePacket = packet as S2C_LeaveGame;
        
    }

    public static void S2C_SpawnHandler(PacketSession session, IMessage packet)
    {
        S2C_Spawn spawnPacket = packet as S2C_Spawn;

        Log.Info("Spawn");

    }

    public static void S2C_DespawnHandler(PacketSession session, IMessage packet)
    {
        S2C_Despawn despawnPacket = packet as S2C_Despawn;
    }

    public static void S2C_MoveHandler(PacketSession session, IMessage packet)
    {
        S2C_Move movePacket = packet as S2C_Move;
        
    }

    public static void S2C_SkillHandler(PacketSession session, IMessage packet)
    {
        S2C_Skill skillPacket = packet as S2C_Skill;
        
    }

    public static void S2C_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S2C_ChangeHp changeHpPacket = packet as S2C_ChangeHp;
       
    }

    public static void S2C_DeadHandler(PacketSession session, IMessage packet)
    {
        S2C_Dead deadPacket = packet as S2C_Dead;
        
    }

    public static void S2C_ChatHandler(PacketSession session, IMessage packet)
    {
        S2C_Chat chatPacket = packet as S2C_Chat;

    }

    public static void S2C_ChangeStatHandler(PacketSession session, IMessage packet)
    {
        S2C_ChangeStat changeStatPacket = (S2C_ChangeStat)packet;
    }
}