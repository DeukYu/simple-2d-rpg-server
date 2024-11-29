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
       DbTransaction.SavePlayerStatus_AllInOne(this, _zone);
    }
}
