using DummyClient.Session;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using ServerCore;
using System.Reflection;
using Google.Protobuf.Common;

namespace DummyClient.Packet;

class PacketHandler
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

    public static void S2C_PingHandler(PacketSession session, IMessage packet)
    {
        C2S_Ping pingPacket = new C2S_Ping();
        var serverSession = (ServerSession)session;
        Log.Info("Ping!");
        serverSession.Send(pingPacket);
    }

    // 서버 연결 되었을 때
    public static void S2C_ConnectedHandler(PacketSession session, IMessage packet)
    {
        C2S_Login loginPacket = new C2S_Login();
        var serverSession = (ServerSession)session;
        loginPacket.AccountName = $"DummyClient_{serverSession.DummyId.ToString("0000")}";
        serverSession.Send(loginPacket);
    }

    // 로그인 + 캐릭터 목록
    public static void S2C_LoginHandler(PacketSession session, IMessage packet)
    {
        S2C_Login loginPacket = (S2C_Login)packet;
        ServerSession serverSession = (ServerSession)session;

        // TODO : Lobby UI에서 캐릭터 보여주고, 선택할 수 있도록 만든 후, 수정 예정
        if (loginPacket.Players == null || loginPacket.Players.Count == 0)
        {
            C2S_CreatePlayer createPacket = new C2S_CreatePlayer();
            createPacket.Name = $"Player_{serverSession.DummyId.ToString("0000")}";
            serverSession.Send(createPacket);
        }
        else
        {
            // TODO : 무조건 첫번째 로그인 -> 추후 수정 필요
            LobbyPlayerInfo info = loginPacket.Players[0];
            C2S_EnterGame enterGamePacket = new C2S_EnterGame();
            enterGamePacket.Name = info.Name;
            serverSession.Send(enterGamePacket);
        }
    }

    public static void S2C_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S2C_CreatePlayer createPlayerPacket = (S2C_CreatePlayer)packet;
        ServerSession serverSession = (ServerSession)session;

        if (createPlayerPacket.Player == null)
        {
            C2S_CreatePlayer createPacket = new C2S_CreatePlayer();
            createPacket.Name = $"Player_{serverSession.DummyId.ToString("0000")}";
            serverSession.Send(createPacket);
        }
        else
        {
            C2S_EnterGame enterGamePacket = new C2S_EnterGame();
            enterGamePacket.Name = createPlayerPacket.Player.Name;
            serverSession.Send(enterGamePacket);
        }
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