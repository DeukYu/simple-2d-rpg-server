using Google.Protobuf.Enum;

namespace WebServer.Services;

public interface IAccountService
{
    Task<int> CreateAccountAsync(string accountName, string password);
    Task<int> LoginAccountAsync(string accountName, string password);
}

public class AccountService : IAccountService
{
    public async Task<int> CreateAccountAsync(string accountName, string password)
    {
        //var accountInfo = await _account.AccountInfo
        //    .AsNoTracking()
        //    .Where(x => x.AccountName == accountName)
        //    .SingleAsync();
        //if (accountInfo != null)
        //{
        //    return (int)ErrorType.AlreadyExistName;
        //}
        //accountInfo = new AccountInfo
        //{
        //    AccountName = accountName
        //};
        //await _account.AccountInfo.AddAsync(accountInfo);
        //await _account.SaveChangesAsync();
        return (int)ErrorType.Success;
    }

    public async Task<int> LoginAccountAsync(string accountName, string password)
    {
        //var accountInfo = await _account.AccountInfo
        //    .AsNoTracking()
        //    .Where(x => x.AccountName == accountName)
        //    .SingleAsync();
        //if (accountInfo == null)
        //{
        //    // 계정이 없으면 생성
        //    accountInfo = new AccountInfo
        //    {
        //        AccountName = accountName
        //    };
        //    await _account.AccountInfo.AddAsync(accountInfo);
        //    await _account.SaveChangesAsync();
        //}
        return (int)ErrorType.Success;
    }
}
