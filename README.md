# simple-2d-mmorpg-server
- C# 서버를 제작중입니다.
- Client : https://github.com/kdh0794/GuardianOfNature2D  

## 환경설정
- Visual Studio 2022
- C# .Net 8.0

## Nuget Package
- NLog
```
dotnet add package NLog
dotnet add package NLog.Config
```
- ProtoBuf (3.28.3)
```
dotnet add package Google.Protobuf
```

# Server (CS_Server)
## ServerCore
- 서버 핵심 기능들 구현
- 현재는 기능이 많지 않아 ServerCore에 모든 Common 부분들을 관리하고 있습니다.
### Network
- 서버 네트워크 통신 관련 기능
    - Connector : Client에서 Connect 하기 위해 사용
    - Listener : Server에서 Client 연결을 확인하기 위한 용도
    - Session
    - RecvBuffer
    - ~~SendBuffer : protobuf 로 인하여 필요 없어져서 삭제~~

### Job
- 서버 패킷 관련 한번에 모아 보내기 위한 부분과 일정 시간마다 패킷을 전송할 수 있도록 하는 기능
    - JobQueue
    - JobTimer

### Logger
- 로그 관련 처리하기 위하여 NLog 라이브러리를 맵핑하여 사용
    - LoggerBase
    - Log
    - NLogLogger

### Util
- 가볍게 쓸 수 있는 Util 파일 관련 
    - DnsUtil : 로컬 IP 주소 관련
    - AtomicFlag : Flag가 필요할 경우, thread safe를 위해 사용

<details>
<summary> AtomicFlag </summary>

- 코드 내에서 Thread Safe하게 상태를 저장하고 관리할 수 있도록 간단한 구조로 Mult-Thread 환경에서 동기화 문제를 해결하기 위하여 사용하였습니다.

```
public sealed class AtomicFlag
{
    private volatile int _flag = 0;

    public static implicit operator bool(AtomicFlag target)
    {
        // true = 1 이고 false = 0 이기 때문에 1이면 true를 반환
        return target._flag == 1;
    }
    public bool Set()
    {
        // 현재 false 일 경우, true로 바꾸고 false를 반환
        return Interlocked.CompareExchange(ref _flag, 1, 0) == 0;
    }

    public void Release()
    {
        // false로 셋팅
        Interlocked.Exchange(ref _flag, 0);
    }
}

```

</details>

## CS_Server
- 기본 게임 서버를 위한 로직 처리

### Session
- Session 관련
    - SessionManager : Client Session 관리
    - ClientSession

### Packet
- Packet 관련
    - Common, Enum, Protocol : proto 에서 생성한 파일
    - ServerPacketManager : Server Packet 관리
    - PacketHandler : Recv Packet 처리
    
<details> 
<summary>PacketManager - Packet Register</summary>

- C# reflection을 통하여 기존 MsgId 를 통하여 Packet Register를 하였으나, Packet이 늘어남에 따라 Enum MsgId를 추가해야하는 번거로움으로 인하여 MsgId 값을 Msg Name을 통하여 SHA256 을 통한 해쉬값을 ushort 값으로 받아와 MsgId로 사용하도록 하였습니다.

```
private void Register()
{
    // 현재 어셈블리에서 IMessage를 구현한 비추상 타입 가져오기
    var packetTypes = Assembly.GetExecutingAssembly().GetTypes()
        .Where(x => typeof(IMessage).IsAssignableFrom(x) && !x.IsAbstract);

    foreach (var packetType in packetTypes)
    {
        // Descriptor를 가져오고, null 체크
        var descriptor = GetMessageDescriptor(packetType);
        if (descriptor == null)
            continue;

        // 메시지 이름으로 메시지 ID 계산
        ushort messageId = ComputeMessageId(descriptor.Name);
        if (_onRecv.ContainsKey(messageId))
        {
            Log.Error($"Already registered message: {messageId}");
            continue;
        }

        // MakePacket<T>를 호출하는 델리게이트 생성
        var makePacketAction = CreateMakePacketAction(packetType);

        // 델리게이트 및 핸들러 등록 (Send 할 때, MsgId를 가져오기 위한 typeToMsgId 등록)
        _onRecv.Add(messageId, makePacketAction);
        RegisterHandler(messageId, packetType);
        _typeToMsgId.Add(packetType, messageId);
    }
}

private ushort ComputeMessageId(string messageName)
{
    using var sha256 = SHA256.Create();
    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(messageName));
    return BitConverter.ToUInt16(hash, 0);
}
```
</details>

## Web Server
- 로그인 기능 및 웹 API 관련을 위한 서버
