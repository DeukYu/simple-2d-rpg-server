using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using Shared;

namespace CS_Server;

public partial class Zone : JobSerializer
{
    private bool IsValidMove(Player player, PositionInfo movePosInfo)
    {
        var currentPos = new Vector2Int(player.PosInfo.PosX, player.PosInfo.PosY);
        var destPos = new Vector2Int(movePosInfo.PosX, movePosInfo.PosY);

        if (currentPos == destPos)
        {
            return true;
        }

        return Map.CanGo(destPos);
    }

    private void UpdatePlayerPosition(Player player, PositionInfo movePosInfo)
    {
        ObjectInfo playerInfo = player.Info;
        playerInfo.PosInfo.State = movePosInfo.State;
        playerInfo.PosInfo.MoveDir = movePosInfo.MoveDir;
        Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));
    }

    public void HandleMove(Player player, C2S_Move packet)
    {
        if (player == null)
        {
            Log.Error("HandleMove player is null");
            return;
        }

        if (IsValidMove(player, packet.PosInfo) == false)
        {
            return;
        }

        PositionInfo movePosInfo = packet.PosInfo;
        ObjectInfo playerInfo = player.Info;

        // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
        if (movePosInfo.PosX != playerInfo.PosInfo.PosX || movePosInfo.PosY != playerInfo.PosInfo.PosY)
        {
            if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
            {
                return;
            }
        }

        UpdatePlayerPosition(player, movePosInfo);

        S2C_Move res = new S2C_Move
        {
            ObjectId = player.Info.ObjectId,
            PosInfo = packet.PosInfo,
        };
        BroadCast(player.CellPos,res);
    }

    public void HandleSkill(Player player, C2S_Skill packet)
    {
        if (player == null)
        {
            Log.Error("HandleSkill player is null");
            return;
        }

        if (CanUseSkill(player) == false)
        {
            return;
        }

        var skillData = GetSkillData(packet.SkillInfo.SkillId);
        if (skillData == null)
        {
            Log.Error("HandleSkill skillData is null");
            return;
        }

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

        S2C_Skill res = new S2C_Skill
        {
            ObjectId = player.Info.ObjectId,
            SkillInfo = new SkillInfo
            {
                SkillId = 1,
            }
        };
        BroadCast(player.CellPos, res);
    }

    private bool CanUseSkill(Player player)
    {
        ObjectInfo info = player.Info;
        if (info.PosInfo.State != CreatureState.Idle)
        {
            Log.Error("HandleSkill player is not idle");
            return false;
        }

        info.PosInfo.State = CreatureState.Skill;

        return true;
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
