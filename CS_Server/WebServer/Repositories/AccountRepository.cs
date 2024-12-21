using Microsoft.EntityFrameworkCore;
using Shared.DB;

namespace WebServer.Repositories;

public interface IAccountRepository
{
    Task<AccountInfo?> GetAccountByNameAsync(string accountName);
    Task<bool> IsAccountExistAsync(string accountName);
    Task CreateAccountAsync(string accountName, string password);
    Task SaveChangesAsync();
}

public class AccountRepository : IAccountRepository
{
    private readonly AccountDB _context;
    public AccountRepository(AccountDB context)
    {
        _context = context;
    }
    public async Task<AccountInfo?> GetAccountByNameAsync(string accountName)
    {
        return await _context.AccountInfo
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountName == accountName);
    }

    public async Task<bool> IsAccountExistAsync(string accountName)
    {
        return await _context.AccountInfo.AnyAsync(x => x.AccountName == accountName);
    }

    public async Task CreateAccountAsync(string accountName, string password)
    {
        var account = new AccountInfo
        {
            AccountName = accountName,
            Password = password
        };

        await _context.AccountInfo.AddAsync(account);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesExAsync();
    }
}
