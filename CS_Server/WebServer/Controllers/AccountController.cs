using Google.Protobuf.Enum;
using Google.Protobuf.WebProtocol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace WebServer;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountDB _account;

    public AccountController(AccountDB context)
    {
        _account = context;
    }

    [HttpPost]
    [Route("login")]
    public async Task<LoginAccountRes> LoginAccount(LoginAccountReq req)
    {
        // TODO : 현재 DB가 없는 상태에선 무조건 OK를 보낸다.
        // TODO : 나중에 DB 연동하면서 로직을 변경해야 한다.
        // TODO : 로그인 계정이 존재하지 않더라도 계정을 생성하고 OK 패킷을 보내도록 처리한다.

        var accountInfo = await _account.AccountInfo.Where(x => x.AccountName == req.AccountName).SingleAsync();
        if (accountInfo == null)
        {
            // 계정이 없으면 생성
            accountInfo = new AccountInfo
            {
                AccountName = req.AccountName
            };

            await _account.AccountInfo.AddAsync(accountInfo);
            await _account.SaveChangesAsync();
        }


        return new LoginAccountRes
        {
            PlayerId = accountInfo.Id,
            Result = (int)ErrorType.Success
        };
    }
}
