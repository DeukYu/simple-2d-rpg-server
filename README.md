# simple-2d-rpg-server
- C# 서버를 제작중입니다.
- 포트폴리오 작업과 함께 README 파일도 업데이트 하고 있습니다.
- Client : https://github.com/kdh0794/GuardianOfNature2D

## 환경설정
- Visual Studio 2022
- C# .Net 8.0
- MySQL

## Nuget Package
- NLog : 서버 내에서 로그를 남기기 위하여 사용됩니다.
```
dotnet add package NLog
dotnet add package NLog.Config
```
- ProtoBuf (3.28.3) : 패킷 직렬화를 위해 사용합니다.
```
dotnet add package Google.Protobuf
```
- MySql.EntityFrameworkCore : MySQL 연동을 위해 사용합니다.
```
dotnet add package MySql.EntityFrameworkCore
```
- Newtonsoft.Json : 각종 Data, Config 파일을 가져오기 위해 사용합니다.
```
dotnet add package Newtonsoft.Json
```
## 시스템 구현 내용
- Client-Server Communication
    - 서버-클라이언트  실시간 데이터 전송을 처리할 수 있도록 TCP 소켓을 사용해 통합
- Data serialization and deserialization (Protocol Buffers)
    - 서버-클라이언트 간 데이터 전송 효율성과 플랫폼 독립성 향상을 위해 Protobuf를 사용
- GameData Load
    - Json 형식의 게임 데이터(기획 데이터)를 로드할 수 있는 기능 구현현
- Config Load
    - 서버 내에 필요한 DataBase, Data Path 등 로드할 수 있는 기능 구현
- MapData Load
    - MapData를 읽어와서 처리하도록 구현 (현재 txt파일을 읽어오고 있지만, 추후에 json형식으로 변경 예정)

## 컨텐츠 구현 내용
- 플레이어 이동 동기화 구현
- 플레이어 공격(스킬), Hit 판정 동기화 구현 (Projectile 포함)
- 맵, 오브젝트 충돌 처리 구현
- 채팅 동기화 구현

## Tool 관련
- PacketGenerator : 패킷 관련한 클라이언트, 서버 코드 생성기
- CsvToJson : Csv파일로 된 게임 데이터를 Json 형식으로 바꿔주는 Json 생성기

## 서버 구성도
- ServerCore : Network 및 Util 관련하여 구성
- Shared : Enum, Log, Config, Databae Model 등 서버 공통적으로 사용하는 부분들에 대해 구성
- CS_Server : 기본 게임 로직 관련 구성
- WebServer : 로그인 관련 및 인증 로직으로 구성

# Server (CS_Server)
## ServerCore
- 서버 네트워크 및 기타 핵심 기능들 구현
### Network
- 서버 네트워크 통신 관련 기능

### Job
- 서버 패킷 관련 한번에 모아 보내기 위한 부분과 일정 시간마다 패킷을 전송할 수 있도록 하는 기능

### Util
- 가볍게 쓸 수 있는 Util 파일 관련 

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

## Shared
- 서버에서 공통적으로 사용하는 부분 구현
### Logger
- 로그 관련 처리하기 위하여 NLog 라이브러리를 맵핑하여 사용

### Config
- 서버에서 사용되는 Config 관련

### GameData
- 실제 게임 데이터(기획 데이터) 관련

## CS_Server
- 기본 게임 서버를 위한 로직 처리

### Session
- Session 관련

### Packet
- Packet 관련
    
<details> 
<summary>PacketManager - Packet Register</summary>

- C# 기존 MsgId 를 통하여 Packet Register를 하였으나, Packet이 늘어남에 따라 Enum MsgId를 추가해야하는 번거로움으로 인하여 MsgId 값을 relfection을 이용하여 Msg Name을 통하여 SHA256 을 통한 해쉬값을 ushort 값으로 받아와 MsgId로 사용하도록 하였습니다.

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
        {
            Log.Error($"Descriptor not found for packet type: {packetType.Name}");
            continue;
        }

        // 메시지 이름으로 메시지 ID 계산
        ushort messageId = ComputeMessageId(descriptor.Name);
        if (_onRecv.TryAdd(messageId, CreateMakePacketAction(packetType)) == false)
        {
            Log.Error($"Already registered message: {messageId}");
            continue;
        }

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
- MySQL 연동하여 계정 관련 기능 작업

### Config
- Config 파일 Load 하여 사용할 수 있도록 만들었는데, 현재는 하나의 Config.json 파일에서 로드하여 사용

### Controllers

### DB
- Database 관련 

### Packet
- Packet 관련 
