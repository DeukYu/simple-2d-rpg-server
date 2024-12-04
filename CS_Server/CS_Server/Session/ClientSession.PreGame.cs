using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using ServerCore;
using Shared;

namespace CS_Server;

public partial class ClientSession : PacketSession
{
    public long AccountId { get; private set; }
    public List<LobbyPlayerInfo> lobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

    // 플레이어 로그인 핸들러
    public int HandleLogin(string accountName, out List<LobbyPlayerInfo> lobbyPlayerInfos)
    {
        lobbyPlayerInfos = null;

        // 서버 상태가 로그인 상태가 아니면 리턴
        if (ServerState != ServerState.Login)
        {
            return (int)ErrorType.InvalidServerState;
        }

        // LobbyPlayerInfo를 초기화
        lobbyPlayers.Clear();

        using (var accountDB = new AccountDB())
        {
            var findAccounts = accountDB.AccountInfo
                .Include(x => x.Players)
                .Where(x => x.AccountName == accountName).FirstOrDefault();

            // 찾는 계정이 없으면 새로 생성
            if (findAccounts != null)
            {
                AccountId = findAccounts.Id;

                foreach (var playerInfo in findAccounts.Players)
                {
                    var findStatInfo = accountDB.PlayerStatInfo
                        .Where(x => x.PlayerId == playerInfo.Id).FirstOrDefault();
                    if (findStatInfo == null)
                    {
                        Log.Error("There is no stat info");
                        continue;
                    }
                    var lobbyPlayerInfo = LobbyPlayerInfoFactory.CreateLobbyPlayerInfo(playerInfo, findStatInfo);
                    lobbyPlayerInfos.Add(lobbyPlayerInfo);
                }
            }
            // 찾는 계정이 있으면 해당 계정의 플레이어 정보를 가져옴
            else
            {
                var newAccount = new AccountInfo { AccountName = accountName };
                accountDB.AccountInfo.Add(newAccount);
                if (accountDB.SaveChangesEx() == false)
                {
                    Log.Error("Failed to save changes to the database.");
                    return (int)ErrorType.DbError;
                }

                AccountId = newAccount.Id;
            }
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

        using (AccountDB accountDB = new AccountDB())
        {
            // 같은 유저 이름이 있는지 확인
            var findPlayer = accountDB.PlayerInfo.Where(x => x.PlayerName == playerName).FirstOrDefault();
            if (findPlayer != null)
            {
                return (int)ErrorType.AlreadyExistName;
            }

            // 새로운 플레이어 생성
            var newPlayer = new PlayerInfo
            {
                PlayerName = playerName,
                AccountId = AccountId
            };

            // 플레이어의 스탯 정보 생성
            var initLevel = 1;
            if (DataManager.StatDict.TryGetValue(initLevel, out var stat) == false)
            {
                Log.Error($"There is no stat info. PlayerName:{playerName} Level:{initLevel}");
                return (int)ErrorType.InvalidGameData;
            }
            var newStat = PlayerStatInfoFactory.CreatePlayerStatInfo(newPlayer.Id, stat);

            accountDB.PlayerInfo.Add(newPlayer);
            if (accountDB.SaveChangesEx() == false)
            {
                Log.Error("Failed to save changes to the database.");
                return (int)ErrorType.DbError;
            }

            lobbyPlayerInfo = LobbyPlayerInfoFactory.CreateLobbyPlayerInfo(newPlayer, newStat);
            lobbyPlayers.Add(lobbyPlayerInfo);
        }

        return (int)ErrorType.Success;
    }

    // 플레이어 입장
    public void HandleEnterGame(C2S_EnterGame packet)
    {
        if (ServerState != ServerState.Lobby)
        {
            return;
        }

        LobbyPlayerInfo playerInfo = lobbyPlayers.Find(x => x.Name == packet.Name);

        GamePlayer = ObjectManager.Instance.Add<Player>();
        {
            GamePlayer.PlayerId = playerInfo.PlayerId;
            GamePlayer.Info.Name = playerInfo.Name;
            GamePlayer.Info.PosInfo.State = CreatureState.Idle;
            GamePlayer.Info.PosInfo.MoveDir = MoveDir.Down;
            GamePlayer.Info.PosInfo.PosX = 0;
            GamePlayer.Info.PosInfo.PosY = 0;
            GamePlayer.StatInfo.Level = playerInfo.StatInfo.Level;
            GamePlayer.StatInfo.Hp = playerInfo.StatInfo.Hp;
            GamePlayer.StatInfo.MaxHp = playerInfo.StatInfo.MaxHp;
            GamePlayer.StatInfo.Mp = playerInfo.StatInfo.Mp;
            GamePlayer.StatInfo.MaxMp = playerInfo.StatInfo.MaxMp;
            GamePlayer.StatInfo.Attack = playerInfo.StatInfo.Attack;
            GamePlayer.StatInfo.Exp = 0;
            GamePlayer.StatInfo.TotalExp = playerInfo.StatInfo.TotalExp;
            GamePlayer.Session = this;

            S2C_ItemList itemListPacket = new S2C_ItemList();
            using (var db = new AccountDB())
            {
                var items = db.ItemInfo
                            .Where(x => x.PlayerId == playerInfo.PlayerId)
                            .ToList();

                foreach (var item in items)
                {
                    if (Item.MakeItem(item, out var newItem))
                    {
                        GamePlayer.Inven.Add(newItem);

                        var info = new ItemInfo();
                        info.MergeFrom(newItem.Info);
                        itemListPacket.Items.Add(info);
                    }
                }
            }

            Send(itemListPacket);
        }

        ServerState = ServerState.InGame;

        Zone zone = ZoneManager.Instance.FindZone(1);
        if (zone == null)
        {
            Log.Error("OnConnected: zone is null");
            return;
        }

        zone.Push(zone.EnterZone, GamePlayer);
    }
}
