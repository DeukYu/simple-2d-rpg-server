using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using Shared;

namespace CS_Server;

public class Arrow : Projectile
{
    public GameObject Owner { get; set; }

    private long _nextMoveTick = 0;

    public override GameObject GetOwner()
    {
        return Owner;
    }

    public void Init(GameObject owner, SkillData skillData, ProjectileInfoData projectileInfoData)
    {
        Owner = owner;
        SkillData = skillData;
        ProjectileInfoData = projectileInfoData;
        PosInfo.State = CreatureState.Move;
        PosInfo.MoveDir = owner.Dir;
        PosInfo.PosX = owner.PosInfo.PosX;
        PosInfo.PosY = owner.PosInfo.PosY;
        Speed = projectileInfoData.Speed;
    }

    private bool IsValidState()
    {
        if (Owner == null || Zone == null || SkillData == null || ProjectileInfoData == null)
            return false;
        return true;
    }

    private bool CanMove()
    {
        return _nextMoveTick < Environment.TickCount64;
    }

    private void ScheduleNextMove()
    {
        var moveInterval = (int)(1000 / ProjectileInfoData.Speed);
        Zone.ScheduleJobAfterDelay(moveInterval, Update);
        _nextMoveTick = Environment.TickCount64 + moveInterval;
    }

    private void BroadCastMove()
    {
        S2C_Move movePacket = new S2C_Move
        {
            ObjectId = Id,
            PosInfo = PosInfo
        };
        Zone.BroadCast(CellPos, movePacket);
    }

    private void HandleCollision(Vector2Int destPos)
    {
        var target = Zone.Map.Find(destPos);
        if (target != null)
        {
            var totalDamage = SkillData.Damage + Owner.TotalAttack;
            target.OnDamaged(this, totalDamage);
        }
    }

    public override void Update()
    {
        if (!IsValidState())
            return;

        if (!CanMove())
            return;

        ScheduleNextMove();

        Vector2Int destPos = GetFrontCellPos();
        if (Zone.Map.ApplyMove(this, destPos, isCollision: false))
        {
            CellPos = destPos;  // Update Position
            BroadCastMove();
        }
        else
        {
            HandleCollision(destPos);
            Zone.ScheduleJob(Zone.LeaveZone, this);
        }
    }
}
