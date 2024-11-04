namespace WebServer.Config
{
    [Serializable]
    public class DatabaseConfig
    {
        public string Address { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
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
}
