using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Shared;

public class AccountDB : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured == false)
        {
            optionsBuilder
                .UseMySql(ConfigManager.Instance.DatabaseConfig.GetConnectionConfig(), new MySqlServerVersion(new Version(8, 0, 29)));
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
