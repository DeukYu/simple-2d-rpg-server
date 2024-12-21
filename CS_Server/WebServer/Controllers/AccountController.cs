using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.WebProtocol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DB;

namespace WebServer.Controller;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly AccountDB _account;

    public AccountController(AccountDB context)
    {
        _account = context;
    }

    [HttpPost]
    [Route("create")]
    public async Task<CreateAccountRes> CreateAccountRes(CreateAccountReq req)
    {
        var accountInfo = await _account.AccountInfo
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountName == req.AccountName);
        if (accountInfo == null)
        {
            accountInfo = new AccountInfo
            {
                AccountName = req.AccountName,
                Password = req.Password
            };

            await _account.AccountInfo.AddAsync(accountInfo);
            await _account.SaveChangesAsync();
        }
        else
        {
            return new CreateAccountRes
            {
                Result = (int)ErrorType.AlreadyExistName
            };
        }

        return new CreateAccountRes
        {
            Result = (int)ErrorType.Success
        };
    }

    [HttpPost]
    [Route("login")]
    public async Task<LoginAccountRes> LoginAccount(LoginAccountReq req)
    {
        var accountInfo = await _account.AccountInfo
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountName == req.AccountName && x.Password == req.Password);

        if (accountInfo == null)
        {
            // 계정이 없으면 생성
            accountInfo = new AccountInfo
            {
                AccountName = req.AccountName,
                Password = req.Password
            };

            await _account.AccountInfo.AddAsync(accountInfo);
            await _account.SaveChangesAsync();
        }

        // TODO : 서버 목록 임시
        var serverInfos = new List<ServerInfo>()
        {
            new ServerInfo() { Name = "Server1", Ip = "127.0.0.1", Congestion = 0},
            new ServerInfo() { Name = "Server2", Ip = "127.0.0.1", Congestion = 1},
        };

        return new LoginAccountRes
        {
            Result = (int)ErrorType.Success,
            ServerInfos = { serverInfos }
        };
    }
}
