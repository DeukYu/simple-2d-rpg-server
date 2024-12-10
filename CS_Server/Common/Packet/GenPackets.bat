 protoc --proto_path=../../PacketGenerator --csharp_out=./ Enum.proto Common.proto Protocol.proto WebProtocol.proto
IF ERRORLEVEL 1 PAUSE

REM START ../../PacketGenerator/bin/Debug/PacketGenerator.exe ../../PacketGenerator/Protocol.proto
IF ERRORLEVEL 1 PAUSE
XCOPY /Y Protocol.cs "../../../../simple-2d-rpg-client/Assets/03_Scripts/Packet"
XCOPY /Y Protocol.cs "../../CS_Server/Packet"

XCOPY /Y Common.cs "../../../../simple-2d-rpg-client/Assets/03_Scripts/Packet"
REM XCOPY /Y Common.cs "../../CS_Server/Packet"
REM XCOPY /Y Common.cs "../../WebServer/Packet"
XCOPY /Y Common.cs "../../Shared"

XCOPY /Y Enum.cs "../../../../simple-2d-rpg-client/Assets/03_Scripts/Packet"
XCOPY /Y Enum.cs "../../Shared"

XCOPY /Y WebProtocol.cs "../../../../simple-2d-rpg-client/Assets/03_Scripts/Packet"
XCOPY /Y WebProtocol.cs "../../WebServer/Packet"
REM XCOPY /Y ClientPacketManager.cs "../../../../GuardianOfNature2D/Assets/03_Scripts/Packet"
REM XCOPY /Y ServerPacketManager.cs "../../CS_Server/Packet"