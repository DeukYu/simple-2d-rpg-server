using Microsoft.EntityFrameworkCore;

namespace WebServer;

public class AccountDB : DbContext
{
    public AccountDB(DbContextOptions<AccountDB> options)
    : base(options) // DbContext 기본 생성자 호출
    {
    }
    public DbSet<AccountInfo> AccountInfo { get; set; }
}
