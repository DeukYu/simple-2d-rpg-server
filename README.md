# simple-3d-mmorpg-server

## 환경설정
- Visual Studio 2022
- C# .Net 8.0

## 라이브러리
- NLog
```
dotnet add package NLog
dotnet add package NLog.Config
```

## ServerCore
- 서버 핵심 기능들 구현

### Logger
- 로그 관련 처리하기 위하여 NLog 라이브러리를 맵핑하여 사용 (추후 수정될 소지가 있습니다.)
```
LoggerBase
Log
NLogLogger
```

