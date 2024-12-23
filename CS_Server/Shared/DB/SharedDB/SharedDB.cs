using Microsoft.EntityFrameworkCore;

namespace Shared.DB;

public class SharedDB : DbContext
{
    private string localConnection = "Server=localhost;Port=3306;Database=shared_db;Uid=root;Pwd=1234;";

    // GameServer
    public SharedDB()
    {

    }
    // WebServer
    public SharedDB(DbContextOptions<SharedDB> options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured == false)
        {
            optionsBuilder
                .UseMySql(localConnection/*ConfigManager.Instance.DatabaseConfig.GetConnectionConfig()*/, new MySqlServerVersion(new Version(8, 0, 29)));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TokenInfo>()
            .HasIndex(p => p.AccountId)
            .IsUnique();

        modelBuilder.Entity<ServerConfigInfo>()
            .HasIndex(p => p.Name)
            .IsUnique();
    }
    public DbSet<TokenInfo> TokenInfo { get; set; } = null!;
    public DbSet<ServerConfigInfo> ServerConfigInfo { get; set; } = null!;
}
