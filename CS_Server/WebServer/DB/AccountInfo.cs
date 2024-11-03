using Google.Protobuf.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServer;

[Table("account_info")]
public class AccountInfo
{
    [Key]
    public long Id { get; set; }
    public string AccountName { get; set; }
}

public static class AccountInfoExtensions
{
    public static async Task<AccountInfo?> FindByAccountNameAsync(this DbSet<AccountInfo> account, string accountName)
    {
        return await account
            .Where(x => x.AccountName == accountName)
            .SingleAsync();
    }

    public static async Task<ErrorType> AddAccountAsync(this DbSet<AccountInfo> account, AccountInfo accountInfo)
    {
        var affect = await account.AddAsync(accountInfo);
        return ErrorType.Success;
    }
}