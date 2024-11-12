using Google.Protobuf.Enum;

namespace CS_Server;

public class Projectile : GameObject
{
    public Projectile()
    {
        ObjectType = GameObjectType.Projectile;
    }

    public virtual void Update()
    {
        // TODO
    }
}
