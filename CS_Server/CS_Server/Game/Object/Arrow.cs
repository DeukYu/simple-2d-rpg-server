using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public class Arrow : Projectile
{
    public GameObject? Owner { get; set; }

    long _nextMoveTick = 0;

    public override GameObject GetOwner()
    {
        return Owner;
    }

    public override void Update()
    {
        if (Owner == null || Zone == null || SkillData == null)
            return;

        if (_nextMoveTick >= Environment.TickCount64)
        {
            return;
        }

        if (DataManager.ProjectileInfoDict.TryGetValue(SkillData.ProjectileId, out var projectileInfo) == false)
        {
            Log.Error($"ProjectileInfo is not valid. ProjectileId: {SkillData.ProjectileId}");
            return;
        }

        var tick = (int)(1000 / projectileInfo.Speed);
        Zone.ScheduleJobAfterDelay(tick, Update);
        _nextMoveTick = Environment.TickCount64 + tick;

        Vector2Int destPos = GetFrontCellPos();
        if (Zone.Map.CanGo(destPos))
        {
            CellPos = destPos;

            S2C_Move movePacket = new S2C_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Zone.BroadCast(movePacket);
        }
        else
        {
            var target = Zone.Map.Find(destPos);
            if (target != null)
            {
                var totalDamage = SkillData.Damage + Owner.TotalAttack;
                target.OnDamaged(this, totalDamage);
            }

            // 소멸
            Zone.ScheduleJob(Zone.LeaveZone, this);
        }
    }
}
