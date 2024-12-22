using Google.Protobuf.WebProtocol;
using Microsoft.AspNetCore.Mvc;
using WebServer.Services;

namespace WebServer.Controller;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [Route("create")]
    public async Task<CreateAccountRes> CreateAccountRes(CreateAccountReq req)
    {
        var result = await _accountService.CreateAccountAsync(req.AccountName, req.Password);
        return new CreateAccountRes 
        {
            Result = result
        };
    }

    [HttpPost]
    [Route("login")]
    public async Task<LoginAccountRes> LoginAccount(LoginAccountReq req)
    {
        var (result, serverInfos) = await _accountService.LoginAccountAsync(req.AccountName, req.Password);
        return new LoginAccountRes
        {
            Result = result,
            ServerInfos = { serverInfos }
        };
    }
}
