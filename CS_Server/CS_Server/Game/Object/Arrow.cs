using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public class Arrow : Projectile
{
    public GameObject? Owner { get; set; }

    long _nextMoveTick = 0;

    public override void Update()
    {
        if (Owner == null || _zone == null || SkillData == null)
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

        var tick = (long)(1000 / projectileInfo.Speed);
        _nextMoveTick = Environment.TickCount64 + tick;

        Vector2Int destPos = GetFrontCellPos();
        if (_zone.Map.CanGo(destPos))
        {
            CellPos = destPos;

            S2C_Move movePacket = new S2C_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            _zone.BroadCast(movePacket);

            Log.Info("Move Arrow");
        }
        else
        {
            var target = _zone.Map.Find(destPos);
            if (target != null)
            {
                var totalDamage = SkillData.Damage + Owner.StatInfo.Attack;
                target.OnDamaged(this, totalDamage);
            }

            // 소멸
            _zone.Push(_zone.LeaveZone, this);
        }
    }
}
