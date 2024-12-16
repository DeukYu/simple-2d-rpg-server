using Google.Protobuf.Common;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using ServerCore;
using DummyClient.Session;

namespace DummyClient.Packet;

partial class PacketHandler
{
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
}