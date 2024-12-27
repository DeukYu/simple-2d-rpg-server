using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Shared.DB;

[Table("account_info")]
public class AccountInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public string AccountName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ICollection<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();
}

public static class AccountInfoExtensions
{
    public static AccountInfo? GetAccountInfo(this DbSet<AccountInfo> accountInfo, string accountName)
    {
        return accountInfo
            .Include(x => x.Players)
            .Where(x => x.AccountName == accountName)
            .FirstOrDefault();
    }
    public static AccountInfo? GetAccountInfo(this DbSet<AccountInfo> accountInfo, long accountId)
    {
        return accountInfo
            .Include(x => x.Players)
            .Where(x => x.Id == accountId)
            .FirstOrDefault();
    }

    public static AccountInfo CreateAccount(this DbSet<AccountInfo> accountInfo, string accountName)
    {
        var account = new AccountInfo
        {
            AccountName = accountName
        };
        accountInfo.Add(account);
        return account;
    }
}