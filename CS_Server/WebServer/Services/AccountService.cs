using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Shared.DB;
using WebServer.Repositories;

namespace WebServer.Services;

public interface IAccountService
{
    Task<int> CreateAccountAsync(string accountName, string password);
    Task<(int, List<ServerInfo>)> LoginAccountAsync(string accountName, string password);
}

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    public async Task<int> CreateAccountAsync(string accountName, string password)
    {
        if(await _accountRepository.IsAccountExistAsync(accountName))
        {
            return (int)ErrorType.AlreadyExistName;
        }

        var accountInfo = new AccountInfo
        {
            AccountName = accountName,
            Password = password
        };

        await _accountRepository.CreateAccountAsync(accountName, password);
        await _accountRepository.SaveChangesAsync();

        return (int)ErrorType.Success;
    }

    public async Task<(int, List<ServerInfo>)> LoginAccountAsync(string accountName, string password)
    {
        var accountInfo = await _accountRepository.GetAccountByNameAsync(accountName);
        if (accountInfo == null)
        {
            var result = await CreateAccountAsync(accountName, password);
            if (result != (int)ErrorType.Success)
            {
                return (result, new List<ServerInfo>());
            }
        }

        // TODO : 서버 목록 임시로 반환
        var serverInfos = new List<ServerInfo>()
        {
            new ServerInfo() { Name = "Server1", Ip = "127.0.0.1", Congestion = 0},
            new ServerInfo() { Name = "Server2", Ip = "127.0.0.1", Congestion = 1},
        };

        return ((int)ErrorType.Success, serverInfos);
    }
}
