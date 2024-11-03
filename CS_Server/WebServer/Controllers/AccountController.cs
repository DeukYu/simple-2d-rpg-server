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
    public LoginAccountRes LoginAccount(LoginAccountReq req)
    {
        // TODO : 현재 DB가 없는 상태에선 무조건 OK를 보낸다.
        // TODO : 나중에 DB 연동하면서 로직을 변경해야 한다.
        // TODO : 로그인 계정이 존재하지 않더라도 계정을 생성하고 OK 패킷을 보내도록 처리한다.
        return new LoginAccountRes
        {
            Result = (int)ErrorType.Success
        };
    }
}
