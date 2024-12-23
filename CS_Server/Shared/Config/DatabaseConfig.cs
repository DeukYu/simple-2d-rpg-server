namespace Shared;

[Serializable]
public class DatabaseConfig
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public string Account { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string GetConnectionConfig()
    {
        return $"Server={Address};" +
               $"Port={Port};" +
               $"Database={Name}" +
               $";User={Account};" +
               $"Password={Password};";
    }
}

[Serializable]
public class DatabaseSettings
{
    public List<DatabaseConfig> Databases { get; set; } = new List<DatabaseConfig>();
}