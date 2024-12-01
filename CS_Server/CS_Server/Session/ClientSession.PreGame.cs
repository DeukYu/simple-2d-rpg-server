using Google.Protobuf;
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
    public void HandleLogin(C2S_Login packet)
    {

        if (ServerState != PlayerServerState.ServerStateLogin)
        {
            return;
        }

        lobbyPlayers.Clear();

        using (var db = new AccountDB())
        {
            var findAccounts = db.AccountInfo
                .Include(x => x.Players)
                .Where(x => x.AccountName == packet.UniqueId).FirstOrDefault();
            if (findAccounts != null)
            {
                AccountId = findAccounts.Id;

                S2C_Login res = new S2C_Login();
                foreach (var playerDb in findAccounts.Players)
                {
                    var findStatInfo = db.PlayerStatInfo
                        .Where(x => x.PlayerId == playerDb.Id).FirstOrDefault();

                    if (findStatInfo == null)
                    {
                        Log.Error("There is no stat info");
                        continue;
                    }

                    var lobbyPlayerInfo = new LobbyPlayerInfo
                    {
                        PlayerId = playerDb.Id,
                        Name = playerDb.PlayerName,
                        StatInfo = new StatInfo
                        {
                            Level = findStatInfo.Level,
                            Hp = findStatInfo.Hp,
                            MaxHp = findStatInfo.MaxHp,
                            Mp = findStatInfo.Mp,
                            MaxMp = findStatInfo.MaxMp,
                            Attack = findStatInfo.Attack,
                            Speed = findStatInfo.Speed,
                            TotalExp = findStatInfo.TotalExp
                        }
                    };

                    lobbyPlayers.Add(lobbyPlayerInfo);

                    res.Players.Add(lobbyPlayerInfo);
                }

                res.Result = (int)ErrorType.Success;
                Send(res);

                ServerState = PlayerServerState.ServerStateLobby;
            }
            else
            {
                var newAccount = new AccountInfo
                {
                    AccountName = packet.UniqueId
                };
                db.AccountInfo.Add(newAccount);
                if (db.SaveChangesEx() == false)
                {
                    Send(
                        new S2C_Login
                        {
                            Result = (int)ErrorType.DbError
                        });
                    return;
                }

                AccountId = newAccount.Id;

                S2C_Login res = new S2C_Login();
                res.Result = (int)ErrorType.Success;
                Send(res);

                ServerState = PlayerServerState.ServerStateLobby;
            }
        }
    }
    // 플레이어 생성
    public void HandleCreatePlayer(C2S_CreatePlayer packet)
    {
        // 서버 상태가 로비 상태가 아니면 리턴
        if (ServerState != PlayerServerState.ServerStateLobby)
        {
            S2C_CreatePlayer res = new S2C_CreatePlayer();
            res.Result = (int)ErrorType.InvalidServerState;
            Send(res);
            return;
        }

        using (AccountDB db = new AccountDB())
        {
            // 같은 유저 이름이 있는지 확인
            var findPlayer = db.PlayerInfo.Where(x => x.PlayerName == packet.Name).FirstOrDefault();
            if (findPlayer != null)
            {
                // 이미 이름이 존재합니다.
                S2C_CreatePlayer res = new S2C_CreatePlayer();
                res.Result = (int)ErrorType.AlreadyExistName;
                Send(res);
                return;
            }

            // 새로운 플레이어 생성
            var newPlayer = new PlayerInfo
            {
                PlayerName = packet.Name,
                AccountId = AccountId
            };

            if (DataManager.StatDict.TryGetValue(1, out var stat) == false)
            {
                S2C_CreatePlayer res = new S2C_CreatePlayer();
                res.Result = (int)ErrorType.InvalidGameData;
                Send(res);
                return;
            }
            var newStat = new PlayerStatInfo
            {
                Level = 1,
                Hp = stat.MaxHp,
                MaxHp = stat.MaxHp,
                Mp = stat.MaxMp,
                MaxMp = stat.MaxMp,
                Attack = stat.Attack,
                Speed = stat.Speed,
                TotalExp = 0,
                PlayerId = newPlayer.Id
            };

            db.PlayerInfo.Add(newPlayer);
            if (db.SaveChangesEx() == false)
            {
                S2C_CreatePlayer res = new S2C_CreatePlayer();
                res.Result = (int)ErrorType.DbError;
                Send(res);
                return;
            }

            var lobbyPlayerInfo = new LobbyPlayerInfo
            {
                PlayerId = newPlayer.Id,
                Name = packet.Name,
                StatInfo = new StatInfo
                {
                    Level = stat.Level,
                    Hp = stat.MaxHp,
                    MaxHp = stat.MaxHp,
                    Mp = stat.MaxMp,
                    MaxMp = stat.MaxMp,
                    Attack = stat.Attack,
                    Speed = stat.Speed,
                    TotalExp = 0
                }
            };

            lobbyPlayers.Add(lobbyPlayerInfo);


            S2C_CreatePlayer successres = new S2C_CreatePlayer
            {
                Result = (int)ErrorType.Success,
                Player = lobbyPlayerInfo
            };
            Send(successres);
        }
    }
    public void HandleEnterGame(C2S_EnterGame packet)
    {
        if (ServerState != PlayerServerState.ServerStateLobby)
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
                    if(Item.MakeItem(item, out var newItem))
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

        ServerState = PlayerServerState.ServerStateInGame;

        Zone zone = ZoneManager.Instance.FindZone(1);
        if (zone == null)
        {
            Log.Error("OnConnected: zone is null");
            return;
        }

        zone.Push(zone.EnterZone, GamePlayer);
    }
}
