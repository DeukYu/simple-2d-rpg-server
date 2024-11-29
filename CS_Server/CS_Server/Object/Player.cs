using Google.Protobuf.Enum;
using Microsoft.EntityFrameworkCore;
using NLog;
using ServerCore;
using Shared;

namespace CS_Server;

public class Player : GameObject
{
    public long PlayerId { get; set; }
    public ClientSession? Session { get; set; }

    public Player()
    {
        ObjectType = GameObjectType.Player;
        Speed = 10.0f;
    }

    public override void OnDamaged(GameObject attacker, int damage)
    {
        base.OnDamaged(attacker, damage);
    }

    public override void OnDead(GameObject attacker)
    {
        base.OnDead(attacker);
    }

    public void OnLeaveGame()
    {
        // TODO : 개선 필요
        using (AccountDB db = new AccountDB())
        {
            var playerStatInfo = new PlayerStatInfo();
            playerStatInfo.PlayerId = PlayerId;
            playerStatInfo.Hp = StatInfo.Hp;
            playerStatInfo.Mp = StatInfo.Mp;

            db.Entry(playerStatInfo).State = EntityState.Unchanged;
            db.Entry(playerStatInfo).Property(nameof(playerStatInfo.Hp)).IsModified = true;
            db.Entry(playerStatInfo).Property(nameof(playerStatInfo.Mp)).IsModified = true;
            if (db.SaveChangesEx() == false)
            {
                Log.Error("Failed to save player stat info");
                return;
            }
        }
    }
}
