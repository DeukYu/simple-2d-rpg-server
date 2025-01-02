using Google.Protobuf.Enum;
using Shared;

namespace CS_Server;

public class Projectile : GameObject
{
    public SkillData SkillData { get; set; } = new SkillData();
    public Projectile()
    {
        ObjectType = GameObjectType.Projectile;
    }

    public virtual void Update()
    {
    }
}
