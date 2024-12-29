using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using Shared;
using Shared.DB;
using Shared.Migrations.AccountDBMigrations;

namespace CS_Server;

public partial class ClientSession : PacketSession
{
    public long AccountId { get; private set; }
    public List<LobbyPlayerInfo> lobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

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
    public int HandleLogin(long accountUid, string token, out List<LobbyPlayerInfo> lobbyPlayerInfos)
    {
        lobbyPlayerInfos = new List<LobbyPlayerInfo>();
        if (!IsServerState(ServerState.Login, out int errorType))
            return errorType;
        // LobbyPlayerInfo를 초기화
        lobbyPlayers.Clear();

        // Token 정보 확인한다.
        using(var sharedDB = new SharedDB())
        {
            var tokenInfo = sharedDB.TokenInfo.GetTokenInfo(accountUid);
            if(tokenInfo == null || tokenInfo.Token != token || tokenInfo.Expired < DateTime.UtcNow)
            {
                Log.Error($"Invalid token. AccountUid : {accountUid} Token : {token}");
                return (int)ErrorType.InvalidToken;
            }
        }

        // 계정 정보를 가져온다.
        using(var accountDB = new AccountDB())
        {
            var accountInfo = accountDB.AccountInfo.GetAccountInfo(accountUid);
            if (accountInfo == null)
            {
                Log.Error($"There is no account info. AccountUid:{accountUid}");
                return (int)ErrorType.InvalidAccount;
            }
            AccountId = accountInfo.Id;

            // 캐릭터 목록 
            foreach (var playerInfo in accountInfo.Players)
            {
                var lobbyPlayerInfo = LobbyPlayerInfoFactory.CreateLobbyPlayerInfo(playerInfo);
                lobbyPlayerInfos.Add(lobbyPlayerInfo);
            }
        }
        
        lobbyPlayers = lobbyPlayerInfos;
        ServerState = ServerState.Lobby;
        return (int)ErrorType.Success;
    }
    public int HandleLoginTest(string accountName, out List<LobbyPlayerInfo> lobbyPlayerInfos)
    {
        lobbyPlayerInfos = new List<LobbyPlayerInfo>();

        if (!IsServerState(ServerState.Login, out int errorType))
            return errorType;

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

        if(!IsServerState(ServerState.Lobby, out int errorType))
            return errorType;

        using (var accountDB = new AccountDB())
        {
            if (accountDB.PlayerInfo.IsPlayerNameExist(playerName))
            {
                Log.Error($"Already exist player name. PlayerName:{playerName}");
                return (int)ErrorType.AlreadyExistName;
            }

            // 플레이어의 스탯 정보 생성
            var initLevel = 1;
            if (DataManager.StatDict.TryGetValue(initLevel, out var stat) == false)
            {
                Log.Error($"There is no stat info. PlayerName:{playerName} Level:{initLevel}");
                return (int)ErrorType.InvalidGameData;
            }

            var newPlayer = accountDB.PlayerInfo.CreatePlayer(playerName, AccountId, stat);
            if (accountDB.SaveChangesEx() == false)
            {
                Log.Error("Failed to save changes to the database.");
                return (int)ErrorType.DbError;
            }

            lobbyPlayerInfo = LobbyPlayerInfoFactory.CreateLobbyPlayerInfo(newPlayer);
            lobbyPlayers.Add(lobbyPlayerInfo);
        }
        return (int)ErrorType.Success;
    }

    // 플레이어 입장
    public void HandleEnterGame(string playerName)
    {
        if(!IsServerState(ServerState.Lobby, out _))
            return;

        // Validate Player
        var lobbyPlayerInfo = lobbyPlayers.Find(x => x.Name == playerName);
        if (lobbyPlayerInfo == null)
        {
            Log.Error("HandleEnterGame: lobbyPlayerInfo is null.");
            return;
        }

        // Create GamePlayer
        GamePlayer = ObjectManager.Instance.Add<Player>();
        if (GamePlayer == null)
        {
            Log.Error("OnConnected: GamePlayer is null.");
            return;
        }

        GamePlayer.SetPlayer(this, lobbyPlayerInfo);
        ServerState = ServerState.InGame;

        // Schedule Zone Entry
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
