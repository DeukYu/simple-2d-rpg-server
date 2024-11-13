using Newtonsoft.Json;
using ServerCore;
using Shared;

namespace CS_Server;

public class DataManager
{
    public static Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();
    public static Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();
    public static Dictionary<int, ProjectileInfo> ProjectileInfoDict { get; private set; } = new Dictionary<int, ProjectileInfo>();
    public static void LoadData()
    {
        var statData = LoadJson<StatData, int, Stat>("Stat");
        if (statData == null)
        {
            Log.Error("Failed to load StatData");
            return;
        }
        StatDict = statData.MakeDict();

        var skillData = LoadJson<SkillData, int, Skill>("Skill");
        if (skillData == null)
        {
            Log.Error("Failed to load SkillData");
            return;
        }
        SkillDict = skillData.MakeDict();

        var projectileInfoData = LoadJson<ProjectileData, int, ProjectileInfo>("ProjectileInfo");
        if (projectileInfoData == null)
        {
            Log.Error("Failed to load ProjectileInfoData");
            return;
        }
        ProjectileInfoDict = projectileInfoData.MakeDict();
    }

    static Loader? LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        var text = File.ReadAllText($"{ConfigManager.PathConfig.GameDataPath}/{path}.json");
        return JsonConvert.DeserializeObject<Loader>(text);
    }
}
