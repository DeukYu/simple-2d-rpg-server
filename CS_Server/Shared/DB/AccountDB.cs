using Microsoft.EntityFrameworkCore;

namespace Shared;

public class AccountDB : DbContext
{

    public AccountDB(DbContextOptions<AccountDB> options)
    : base(options) // DbContext 기본 생성자 호출
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL("Server=localhost;Port=3306");
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

    public DbSet<AccountInfo> AccountInfo { get; set; }
    public DbSet<PlayerInfo> PlayerInfo { get; set; }
}
