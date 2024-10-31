# simple-3d-mmorpg-server
- 현재는 C# 서버를 구축하고 있습니다.
- Client : https://github.com/kdh0794/GuardianOfNature2D  
## 환경설정
- Visual Studio 2022
- C# .Net 8.0

## 라이브러리
- NLog
```
dotnet add package NLog
dotnet add package NLog.Config
```

# Server (CS_Server)
## ServerCore
- 서버 핵심 기능들 구현
### Network
- 서버 네트워크 통신 관련 기능
```
Connector
Listener
Session
RecvBuffer
SendBuffer
```
### Job
- 서버 패킷 관련 한번에 모아 보내기 위한 부분과 일정 시간마다 패킷을 전송할 수 있도록 하는 기능
```
JobQueue
JobTimer
```
### Logger
- 로그 관련 처리하기 위하여 NLog 라이브러리를 맵핑하여 사용 (추후 수정될 소지가 있습니다.)
```
LoggerBase
Log
NLogLogger
```
### Util
- 가볍게 쓸 수 있는 Util 파일 관련 
```
DnsUtil : 로컬 IP 주소 관련
```

## DummyClient
- 더미 클라이언트 테스트용

## CS_Server
- 기본 게임 서버를 위한 로직 처리