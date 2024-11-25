using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using ServerCore;
using Shared;

namespace CS_Server;

public partial class ClientSession : PacketSession
{
    public List<LobbyPlayerInfo> lobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();
    public void HandlerLogin(C2S_Login packet)
    {

        if(ServerState != PlayerServerState.ServerStateLogin)
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
                S2C_Login res = new S2C_Login();
                foreach(var playerDb in findAccounts.Players)
                {
                    var findStatInfo = db.PlayerStatInfo
                        .Where(x => x.PlayerId == playerDb.Id).FirstOrDefault();

                    if(findStatInfo == null)
                    {
                        Log.Error("There is no stat info");
                        continue;
                    }

                    var lobbyPlayerInfo = new LobbyPlayerInfo
                    {
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
                db.SaveChanges();
                S2C_Login res = new S2C_Login();
                res.Result = (int)ErrorType.Success;
                Send(res);

                ServerState = PlayerServerState.ServerStateLobby;
            }
        }
    }
}
