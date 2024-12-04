using Google.Protobuf.Enum;

namespace CS_Server;

public class Player : GameObject
{
    public long PlayerId { get; set; }  // DB ID
    public ClientSession? Session { get; set; }
    public Inventory Inven { get; private set; } = new Inventory();

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
       DbTransaction.UpdatePlayerStatus(this, _zone);
    }
}
