using Newtonsoft.Json;
using ServerCore;

namespace Shared;

public sealed class ConfigManager
{
    private static readonly Lazy<ConfigManager> _instance = new Lazy<ConfigManager>(() => new ConfigManager());
    public static ConfigManager Instance => _instance.Value;
    public DatabaseConfig DatabaseConfig { get; private set; } = new DatabaseConfig();
    public PathConfig PathConfig { get; private set; } = new PathConfig();
    private bool _isLoaded = false;

    public void ReloadConfig(string configPath)
    {
        _isLoaded = false;
        LoadConfig(configPath);
    }
    public void LoadConfig(string configPath)
    {
        if (_isLoaded)
            return;

        var text = File.ReadAllText(configPath);

        // DataBase Config
        DatabaseConfig = LoadConfigSection<DatabaseConfig>(text);
        PathConfig = LoadConfigSection<PathConfig>(text);

        _isLoaded = true;
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
