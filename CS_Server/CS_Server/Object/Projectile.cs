using Google.Protobuf.Enum;
using Shared;

namespace CS_Server;

public class Projectile : GameObject
{
    public Skill SkillData { get; set; } = new Skill();
    public Projectile()
    {
        ObjectType = GameObjectType.Projectile;
    }

    public virtual void Update()
    {
        // TODO
    }
}
