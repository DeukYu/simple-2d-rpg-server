using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using Shared;

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

    public void HandleSkill(Player player, C2S_Skill packet)
    {
        if (player == null)
        {
            Log.Error("HandleSkill player is null.");
            return;
        }

        if (player.IsUseableSkill())
        {
            Log.Error("HandleSkill IsUseableSkill is false.");
            return;
        }

        var skillData = GetSkillData(packet.SkillInfo.SkillId);
        if (skillData == null)
        {
            Log.Error("HandleSkill skillData is null");
            return;
        }

        S2C_Skill res = new S2C_Skill
        {
            ObjectId = player.Info.ObjectId,
            SkillInfo = new SkillInfo
            {
                SkillId = 1,
            }
        };
        BroadCast(player.CellPos, res);

        switch (skillData.SkillType)
        {
            case SkillType.Auto:
                HandleAutoSkill(player, skillData);
                break;

            case SkillType.Projectile:
                HandleProjectileSkill(player, skillData);
                break;

            default:
                Log.Error("HandleSkill invalid skill type");
                return;
        }
    }

    private SkillData? GetSkillData(int skillId)
    {
        if (DataManager.SkillDict.TryGetValue(skillId, out var skillData) == false)
        {
            Log.Error($"GetSkillData skillData is null. SkillId{skillId}");
            return null;
        }
        return skillData;
    }

    private void HandleAutoSkill(Player player, SkillData skillData)
    {
        var skillPos = player.GetFrontCellPos(player.PosInfo.MoveDir);
        var target = Map.Find(skillPos);
        if (target != null)
        {
            target.OnDamaged(player, skillData.Damage + player.StatInfo.Attack);
        }
    }

    private void HandleProjectileSkill(Player player, SkillData skillData)
    {
        if (DataManager.ProjectileInfoDict.TryGetValue(skillData.ProjectileId, out var projectileInfo) == false)
        {
            Log.Error($"HandleSkill projectileInfo is null. ProjectileId{skillData.ProjectileId}");
            return;
        }
        var arrow = ObjectManager.Instance.Add<Arrow>();
        if (arrow == null)
        {
            Log.Error("HandleSkill arrow is null");
            return;
        }

        arrow.Owner = player;
        arrow.SkillData = skillData;
        arrow.PosInfo.State = CreatureState.Move;
        arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
        arrow.PosInfo.PosX = player.PosInfo.PosX;
        arrow.PosInfo.PosY = player.PosInfo.PosY;
        arrow.Speed = projectileInfo.Speed;
        ScheduleJob(EnterZone, arrow, false);
    }
}
