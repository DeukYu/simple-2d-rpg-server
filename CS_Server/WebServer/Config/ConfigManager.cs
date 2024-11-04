using Newtonsoft.Json;
using ServerCore;
using WebServer.Config;

namespace WebServer;

public class ConfigManager
{
    // Config는 외부에서 readonly로 접근 가능하도록 설정
    public static DatabaseConfig DatabaseConfig { get; private set; } = new DatabaseConfig();

    public static void LoadConfig()
    {
        string json = File.ReadAllText("../config.json");

        var config = JsonConvert.DeserializeObject<DatabaseConfig>(json);
        if (config == null)
        {
            Log.Error("Failed to load config.json");
            return;
        }

        DatabaseConfig = config;
    }
}
