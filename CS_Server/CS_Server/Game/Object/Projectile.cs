using Google.Protobuf.Enum;
using Shared;

namespace CS_Server;

public class Projectile : GameObject
{
    public SkillData SkillData { get; set; } = new SkillData();
    public ProjectileInfoData ProjectileInfoData { get; set; } = new ProjectileInfoData();
    public Projectile()
    {
        ObjectType = GameObjectType.Projectile;
    }

    public override void Update()
    {
    }
}
