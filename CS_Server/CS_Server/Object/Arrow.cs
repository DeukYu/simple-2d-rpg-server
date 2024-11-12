using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public class Arrow : Projectile
{
    public GameObject Owner { get; set; }

    long _nextMoveTick = 0;

    public override void Update()
    {
        if (Owner == null || _zone == null)
            return;

        if (_nextMoveTick >= Environment.TickCount64)
        {
            return;
        }

        _nextMoveTick = Environment.TickCount64 + 50;

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
                // 피격 판정
            }

            // 소멸
            _zone.LeaveZone(this);
        }
    }
}
