using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Shared.DB;
using WebServer.Repositories;

namespace WebServer.Services;

public interface IAccountService
{
    Task<int> CreateAccountAsync(string accountName, string password);
    Task<LoginAccountResult> LoginAccountAsync(string accountName, string password);
}

// DTO : CreateAccountReq
public class LoginAccountResult
{
    public int ResultCode { get; set; }
    public long AccountId { get; set; }
    public string Token { get; set; } = string.Empty;
    public List<ServerInfo> ServerInfos { get; set; } = new List<ServerInfo>();
}

public class AccountService : IAccountService
{
    private readonly ISharedRepository _sharedRepository;
    private readonly IAccountRepository _accountRepository;
    public AccountService(IAccountRepository accountRepository, ISharedRepository sharedRepository)
    {
        _accountRepository = accountRepository;
        _sharedRepository = sharedRepository;
    }
    public async Task<int> CreateAccountAsync(string accountName, string password)
    {
        if (await _accountRepository.IsAccountExistAsync(accountName))
        {
            return (int)ErrorType.AlreadyExistName;
        }

        var accountInfo = new AccountInfo
        {
            AccountName = accountName,
            Password = password
        };

        await _accountRepository.CreateAccountAsync(accountName, password);

        return (int)ErrorType.Success;
    }

    public async Task<LoginAccountResult> LoginAccountAsync(string accountName, string password)
    {
        var accountInfo = await _accountRepository.GetAccountByNameAsync(accountName);
        if (accountInfo == null)
        {
            var result = await CreateAccountAsync(accountName, password);
            if (result != (int)ErrorType.Success)
            {
                return new LoginAccountResult() { ResultCode = result };
            }
        }

        var token = Guid.NewGuid().ToString();
        var expired = DateTime.UtcNow.AddMinutes(5);

        var tokenInfo = await _sharedRepository.GetTokenByAccountId(accountInfo.Id);
        if (tokenInfo != null)
        {
            await _sharedRepository.UpdateTokenAsync(tokenInfo, token, expired);
        }
        else
        {
            tokenInfo = await _sharedRepository.CreateTokenAsync(accountInfo.Id, token, expired);
        }

        var serverConfigInfos = _sharedRepository.GetServerConfigInfosAsync().Result;
        var serverInfos = new List<ServerInfo>();
        foreach (var serverConfigInfo in serverConfigInfos)
        {
            serverInfos.Add(new ServerInfo()
            {
                Name = serverConfigInfo.Name,
                IpAddress = serverConfigInfo.IpAddress,
                Port = serverConfigInfo.Port,
                Congestion = serverConfigInfo.Congestion
            });
        }

        return new LoginAccountResult
        {
            ResultCode = (int)ErrorType.Success,
            AccountId = accountInfo.Id,
            Token = token,
            ServerInfos = serverInfos
        };
    }
}
