using Google.Protobuf.Common;
using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public partial class Zone : JobSerializer
{
    public void HandleMove(Player player, PositionInfo positionInfo)
    {
        if (player == null)
        {
            Log.Error("HandleMove player is null.");
            return;
        }

        if (player.IsValidMove(positionInfo) == false)
        {
            Log.Error("HandleMove IsValidMove is false.");
            return;
        }

        player.UpdatePosition(positionInfo);

        S2C_Move res = new S2C_Move
        {
            ObjectId = player.Info.ObjectId,
            PosInfo = positionInfo,
        };
        BroadCast(player.CellPos,res);
    }

    public void HandleSkill(Player player, SkillInfo skillInfo)
    {
        if (player == null)
            return;

        if (player.IsUseableSkill() == false)
            return;

        if (DataManager.SkillDict.TryGetValue(skillInfo.SkillId, out var skillData) == false)
        {
            Log.Error($"GetSkillData skillData is null. SkillId: {skillInfo.SkillId}");
            return;
        }

        S2C_Skill res = new S2C_Skill
        {
            ObjectId = player.Id,
            SkillInfo = new SkillInfo
            {
                SkillId = skillData.Id,
            }
        };
        BroadCast(player.CellPos, res);

        player.HandlerSkill(skillData);
    }
}
