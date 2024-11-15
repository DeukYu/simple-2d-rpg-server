using Google.Protobuf.Enum;
using ServerCore;

namespace CS_Server;

public class Player : GameObject
{
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
}
