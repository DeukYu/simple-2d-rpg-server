using Google.Protobuf.Enum;
using Google.Protobuf.WebProtocol;
using Microsoft.AspNetCore.Mvc;

namespace WebServer;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet]
    [Route("login")]
    public LoginAccountRes LoginAccount(LoginAccountReq id)
    {
        // TODO : 현재 DB가 없는 상태에선 무조건 OK를 보낸다.
        return new LoginAccountRes
        {
            Result = (int)ErrorType.Success
        };
    }
}
