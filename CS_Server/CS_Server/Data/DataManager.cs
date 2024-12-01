using Shared;

namespace CS_Server;

public class DataManager
{
    public static Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();
    public static Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();
    public static Dictionary<int, ProjectileInfo> ProjectileInfoDict { get; private set; } = new Dictionary<int, ProjectileInfo>();
    public static Dictionary<int, ItemData> ItemDataDict { get; private set; } = new Dictionary<int, ItemData>();
    public static void LoadData()
    {
        StatDict = DataLoader.Load<StatData, int, Stat>("Stat");
        SkillDict = DataLoader.Load<SkillData, int, Skill>("Skill");
        ProjectileInfoDict = DataLoader.Load<ProjectileData, int, ProjectileInfo>("ProjectileInfo");
        ItemDataDict = DataLoader.Load<ItemDataLoader, int, ItemData>("Item");
    }
}