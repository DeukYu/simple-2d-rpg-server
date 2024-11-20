using Newtonsoft.Json;
using ServerCore;
using Shared;

namespace CS_Server;

public class ConfigManager
{
    public static DatabaseConfig DatabaseConfig { get; private set; } = new DatabaseConfig();
    public static PathConfig PathConfig { get; private set; } = new PathConfig();

    public static void LoadConfig()
    {
        var text = File.ReadAllText("../../../../config.json");

        // DataBase Config
        DatabaseConfig = LoadConfigSection<DatabaseConfig>(text);
        PathConfig = LoadConfigSection<PathConfig>(text);
    }

    public static T LoadConfigSection<T>(string jsonText) where T : new()
    {
        var section = JsonConvert.DeserializeObject<T>(jsonText);
        if (section == null)
        {
            Log.Error($"Failed to load {typeof(T).Name} config");
            return new T();
        }
        return section;
    }
}
