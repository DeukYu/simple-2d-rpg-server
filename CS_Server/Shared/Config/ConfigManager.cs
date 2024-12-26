using Newtonsoft.Json;
using ServerCore;
using Shared.Config;

namespace Shared;

public sealed class ConfigManager
{
    private enum LoadResult
    {
        NotLoaded,
        Success,
        Failed
    }

    private LoadResult _loadResult = LoadResult.NotLoaded;

    private static readonly Lazy<ConfigManager> _instance = new Lazy<ConfigManager>(() => new ConfigManager());
    public static ConfigManager Instance => _instance.Value;
    public DatabaseConfig AccountDbConfig { get; private set; } = new DatabaseConfig();
    public DatabaseConfig SharedDbConfig { get; private set; } = new DatabaseConfig();
    public PathConfig PathConfig { get; private set; } = new PathConfig();
    public ServerConfig ServerConfig { get; private set; } = new ServerConfig();

    public void ReloadConfig(string configPath)
    {
        _loadResult = LoadResult.NotLoaded;
        LoadConfig(configPath);
    }
    public void LoadConfig(string configPath)
    {
        if (_loadResult == LoadResult.Success)
        {
            Log.Warn("Config already loaded.");
            return;
        }

        try
        {
            var text = File.ReadAllText(configPath);

            // DataBase Config
            if (TryLoadDatabaseConfig(text, "account_db", out var accountDb) == false ||
                TryLoadDatabaseConfig(text, "shared_db", out var sharedDb) == false)
                return;

            AccountDbConfig = accountDb;
            SharedDbConfig = sharedDb;

            PathConfig = LoadConfigSection<PathConfig>(text);
            ServerConfig = LoadConfigSection<ServerConfig>(text);

            Log.Info("Config loaded.");
            _loadResult = LoadResult.Success;
        }
        catch (Exception e)
        {
            Log.Error($"Failed to load config: {e.Message}");
            _loadResult = LoadResult.Failed;
        }
    }

    private bool TryLoadDatabaseConfig(string jsonText, string dbName, out DatabaseConfig config)
    {
        config = new DatabaseConfig();

        var databaseSettings = LoadConfigSection<DatabaseSettings>(jsonText);
        if (databaseSettings == null)
        {
            Log.Error("Failed to load DatabaseSettings.");
            return false;
        }

        var dbConfig = databaseSettings.Databases.FirstOrDefault(db => db.Name == dbName);
        if (dbConfig == null)
        {
            Log.Error($"Database configuration for {dbName} not found.");
            return false;
        }

        config = dbConfig;
        return true;
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
