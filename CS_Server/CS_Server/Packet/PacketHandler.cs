using ServerCore;

namespace CS_Server;

public class PacketHandler
{
    public static void C2S_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C2S_PlayerInfoReq req = packet as C2S_PlayerInfoReq;

        Log.Info($"PlayerInfoReq => {req.playerId} {req.name}");

        foreach (C2S_PlayerInfoReq.Skill skill in req.skills)
        {
            Log.Info($"Skill Info => {skill.id} {skill.level} {skill.duration}");
        }
    }
}
