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
                .UseMySQL(ConfigManager.Instance.DatabaseConfig.GetConnectionConfig())
                .UseLoggerFactory(_logger);
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


    public DbSet<AccountInfo> AccountInfo { get; set; }
    public DbSet<PlayerInfo> PlayerInfo { get; set; }
}
