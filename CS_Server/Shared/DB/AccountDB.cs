using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Shared.DB;

public class AccountDB : DbContext
{
    private string localConnection = "Server=localhost;Port=3306;Database=account_db;Uid=root;Pwd=deukyu1874!;";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured == false)
        {
            optionsBuilder
                .UseMySql(localConnection/*ConfigManager.Instance.DatabaseConfig.GetConnectionConfig()*/, new MySqlServerVersion(new Version(8, 0, 29)));
            //.UseLoggerFactory(_logger);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountInfo>()
            .HasIndex(p => p.AccountName)
            .IsUnique();

        modelBuilder.Entity<PlayerInfo>()
            .HasIndex(p => p.PlayerName)
            .IsUnique();
    }

    static readonly ILoggerFactory _logger = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });


    public DbSet<AccountInfo> AccountInfo { get; set; } = null!;
    public DbSet<PlayerInfo> PlayerInfo { get; set; } = null!;
    public DbSet<PlayerStatInfo> PlayerStatInfo { get; set; } = null!;
    public DbSet<PlayerItemInfo> ItemInfo { get; set; } = null!;
}
