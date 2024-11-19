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
        var databaseConfig = JsonConvert.DeserializeObject<DatabaseConfig>(text);
        if(databaseConfig == null)
        {
            Log.Error("Failed to load config.json");
            return;
        }
        DatabaseConfig = databaseConfig;

        // Path Config
        var pathConfig = JsonConvert.DeserializeObject<PathConfig>(text);
        if (pathConfig == null)
        {
            Log.Error("Failed to load config.json");
            return;
        }
        PathConfig = pathConfig;
    }
}
