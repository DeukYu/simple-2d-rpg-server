using Newtonsoft.Json;
using ServerCore;
using WebServer.Config;

namespace WebServer;

public class ConfigManager
{
    public static DatabaseConfig DatabaseConfig { get; private set; } = new DatabaseConfig();
    public static void LoadConfig()
    {
        string json = File.ReadAllText("../config.json");

        var config = JsonConvert.DeserializeObject<DatabaseConfig>(json);
        if (config == null)
        {
            Log.Error("Failed to load Config.json");
            return; 
        }

        DatabaseConfig = config; 
    }
}
