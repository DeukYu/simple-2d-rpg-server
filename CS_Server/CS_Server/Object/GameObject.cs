using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public class GameObject
{
    public Zone? _zone;
    public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
    public ObjectInfo Info { get; set; } = new ObjectInfo();
    public PositionInfo PosInfo { get; private set; } = new PositionInfo();
    public StatInfo StatInfo { get; private set; } = new StatInfo();
    public float Speed
    {
        get { return StatInfo.Speed; }
        set { StatInfo.Speed = value; }
    }
    public long Id
    {
        get { return Info.ObjectId; }
        set { Info.ObjectId = value; }
    }

    public GameObject()
    {
        Info.PosInfo = PosInfo;
        Info.StatInfo = StatInfo;
    }

    public Vector2Int CellPos
    {
        get
        {
            return new Vector2Int(PosInfo.PosX, PosInfo.PosY);
        }
        set
        {
            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
        }
    }

    public Vector2Int GetFrontCellPos()
    {
        return GetFrontCellPos(PosInfo.MoveDir);
    }

    public Vector2Int GetFrontCellPos(MoveDir dir)
    {
        Vector2Int cellPos = CellPos;

        switch (dir)
        {
            case MoveDir.Up:
                cellPos += Vector2Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector2Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector2Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector2Int.right;
                break;

        }
        return cellPos;
    }

    public virtual void OnDamaged(GameObject attacker, int damage)
    {
        StatInfo.Hp -= damage;
        StatInfo.Hp = Math.Max(StatInfo.Hp, 0);

        S2C_ChangeHp changeHpPacket = new S2C_ChangeHp();
        changeHpPacket.ObjectId = Id;
        changeHpPacket.Hp = StatInfo.Hp;
        _zone.BroadCast(changeHpPacket);

        if (StatInfo.Hp <= 0)
        {
            OnDead(attacker);
        }
    }
    public virtual void OnDead(GameObject attacker)
    {
        S2C_Dead deadPacket = new S2C_Dead();
        deadPacket.ObjectId = Id;
        deadPacket.AttackerId = attacker.Id;
        _zone.BroadCast(deadPacket);

        var zone = _zone;
        zone.LeaveZone(this);

        StatInfo.Hp = StatInfo.MaxHp;
        PosInfo.State = CreatureState.Idle;
        PosInfo.MoveDir = MoveDir.Down;
        PosInfo.PosX = 0;
        PosInfo.PosY = 0;

        zone.EnterZone(this);
    }
}
