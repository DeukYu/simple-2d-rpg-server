using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using ServerCore;
using Shared.DB;

namespace CS_Server;

public partial class ClientSession : PacketSession
{
    public long AccountId { get; private set; }
    public List<LobbyPlayerInfo> lobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

    private AccountInfo? GetAccountInfo(string accountName)
    {
        using (var accountDB = new AccountDB())
        {
            return accountDB.AccountInfo
                .Include(x => x.Players)
                .Where(x => x.AccountName == accountName).FirstOrDefault();
        }
    }

    private bool CreateAccountInfo(string accountName, out AccountInfo account)
    {
        using (var accountDB = new AccountDB())
        {
            account = new AccountInfo { AccountName = accountName };
            accountDB.AccountInfo.Add(account);
            if (accountDB.SaveChangesEx() == false)
            {
                Log.Error("Failed to save changes to the database.");
                return false;
            }
        }
        return true;
    }

    // 플레이어 로그인 핸들러 : 로그인 + 캐릭터 목록
    public int HandleLogin(string accountName, out List<LobbyPlayerInfo> lobbyPlayerInfos)
    {
        lobbyPlayerInfos = new List<LobbyPlayerInfo>();

        // 서버 상태가 로그인 상태가 아니면 리턴
        if (ServerState != ServerState.Login)
        {
            return (int)ErrorType.InvalidServerState;
        }

        // LobbyPlayerInfo를 초기화
        lobbyPlayers.Clear();

        // 계정 정보를 가져온다.
        var accountDB = new AccountDB();
        var accountInfo = accountDB.AccountInfo.GetAccountInfo(accountName);
        if (accountInfo == null)
        {
            // TODO : 임시적으로 계정 정보가 없으면 생성하도록 한다.
            if (CreateAccountInfo(accountName, out accountInfo) == false)
            {
                Log.Error($"Failed to create account info. AccountName{accountName}");
                return (int)ErrorType.DbError;
            }

            // TODO : 정상 로직
            //Log.Error($"There is no account info. AccountName:{accountName}");
            //return (int)ErrorType.InvalidAccount;
        }

        AccountId = accountInfo.Id;

        // 캐릭터 목록 
        foreach (var playerInfo in accountInfo.Players)
        {
            var lobbyPlayerInfo = LobbyPlayerInfoFactory.CreateLobbyPlayerInfo(playerInfo);
            lobbyPlayerInfos.Add(lobbyPlayerInfo);
        }

        lobbyPlayers = lobbyPlayerInfos;
        ServerState = ServerState.Lobby;
        return (int)ErrorType.Success;
    }

    // 플레이어 생성
    public int HandleCreatePlayer(string playerName, out LobbyPlayerInfo lobbyPlayerInfo)
    {
        lobbyPlayerInfo = null;
        if (ServerState != ServerState.Lobby)
        {
            return (int)ErrorType.InvalidServerState;
        }

        var accountDB = new AccountDB();

        if (accountDB.PlayerInfo.IsPlayerNameExist(playerName))
        {
            Log.Error($"Already exist player name. PlayerName:{playerName}");
            return (int)ErrorType.AlreadyExistName;
        }

        // 새로운 플레이어 생성
        var newPlayer = accountDB.PlayerInfo.CreatePlayer(playerName, AccountId);
        if (accountDB.SaveChangesEx() == false)
        {
            Log.Error("Failed to save changes to the database.");
            return (int)ErrorType.DbError;
        }

        // 플레이어의 스탯 정보 생성
        var initLevel = 1;
        if (DataManager.StatDict.TryGetValue(initLevel, out var stat) == false)
        {
            Log.Error($"There is no stat info. PlayerName:{playerName} Level:{initLevel}");
            return (int)ErrorType.InvalidGameData;
        }

        accountDB.PlayerStatInfo.CreatePlayerStat(newPlayer.Id, stat);
        if (accountDB.SaveChangesEx() == false)
        {
            Log.Error("Failed to save changes to the database.");
            return (int)ErrorType.DbError;
        }

        lobbyPlayerInfo = LobbyPlayerInfoFactory.CreateLobbyPlayerInfo(newPlayer);
        lobbyPlayers.Add(lobbyPlayerInfo);

        return (int)ErrorType.Success;
    }

    // 플레이어 입장
    public void HandleEnterGame(C2S_EnterGame packet)
    {
        if (ServerState != ServerState.Lobby)
        {
            return;
        }

        var lobbyPlayerInfo = lobbyPlayers.Find(x => x.Name == packet.Name);
        if (lobbyPlayerInfo == null)
        {
            Log.Error("HandleEnterGame: lobbyPlayerInfo is null.");
            return;
        }

        GamePlayer = ObjectManager.Instance.Add<Player>();
        if (GamePlayer == null)
        {
            Log.Error("OnConnected: GamePlayer is null.");
            return;
        }

        GamePlayer.SetPlayer(this, lobbyPlayerInfo);
        ServerState = ServerState.InGame;

        GameLogic.Instance.ScheduleJob(() =>
        {
            Zone zone = GameLogic.Instance.FindZone(1);
            if (zone == null)
            {
                Log.Error("OnConnected: zone is null");
                return;
            }

            zone.ScheduleJob(zone.EnterZone, GamePlayer, true);
        });
    }
}
