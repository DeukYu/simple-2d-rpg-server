 protoc --proto_path=../../PacketGenerator --csharp_out=./ Common.proto Protocol.proto Enum.proto
IF ERRORLEVEL 1 PAUSE

REM START ../../PacketGenerator/bin/Debug/PacketGenerator.exe ../../PacketGenerator/Protocol.proto
IF ERRORLEVEL 1 PAUSE
XCOPY /Y Protocol.cs "../../../../GuardianOfNature2D/Assets/03_Scripts/Packet"
XCOPY /Y Protocol.cs "../../CS_Server/Packet"

XCOPY /Y Common.cs "../../../../GuardianOfNature2D/Assets/03_Scripts/Packet"
XCOPY /Y Common.cs "../../CS_Server/Packet"

XCOPY /Y Enum.cs "../../../../GuardianOfNature2D/Assets/03_Scripts/Packet"
XCOPY /Y Enum.cs "../../CS_Server/Packet"
REM XCOPY /Y ClientPacketManager.cs "../../../../GuardianOfNature2D/Assets/03_Scripts/Packet"
REM XCOPY /Y ServerPacketManager.cs "../../CS_Server/Packet"