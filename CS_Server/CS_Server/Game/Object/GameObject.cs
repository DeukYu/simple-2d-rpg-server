using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public class GameObject
{
    public Zone Zone { get; set; } = null;
    public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
    public ObjectInfo Info { get; set; } = new ObjectInfo();
    public PositionInfo PosInfo { get; private set; } = new PositionInfo();
    public StatInfo StatInfo { get; private set; } = new StatInfo();
    public virtual int TotalAttack { get { return StatInfo.Attack; } }
    public virtual int TotalDefense { get { return 0; } }   // TODO
    public float Speed
    {
        get { return StatInfo.Speed; }
        set { StatInfo.Speed = value; }
    }

    public MoveDir Dir
    {
        get { return PosInfo.MoveDir; }
        set { PosInfo.MoveDir = value; }
    }

    public CreatureState State
    {
        get { return PosInfo.State; }
        set { PosInfo.State = value; }
    }
    public int Hp
    {
        get { return StatInfo.Hp; }
        set { StatInfo.Hp = Math.Clamp(value, 0, StatInfo.MaxHp); }
    }

    public int Id
    {
        get { return Info.ObjectId; }
        set { Info.ObjectId = value; }
    }

    public GameObject()
    {
        Info.PosInfo = PosInfo;
        Info.StatInfo = StatInfo;
    }

    public virtual void Update()
    {

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
    public static MoveDir GetDirFromVec(Vector2Int dir)
    {
        if (dir.x != 0)
            return dir.x > 0 ? MoveDir.Right : MoveDir.Left;
        else if (dir.y != 0)
            return dir.y > 0 ? MoveDir.Up : MoveDir.Down;
        return MoveDir.Down;
    }

    public virtual void OnDamaged(GameObject attacker, int damage)
    {
        if (Zone == null)
        {
            return;
        }

        damage = Math.Max(damage - TotalDefense, 0);

        StatInfo.Hp -= damage;
        StatInfo.Hp = Math.Max(StatInfo.Hp, 0);

        S2C_ChangeHp changeHpPacket = new S2C_ChangeHp();
        changeHpPacket.ObjectId = Id;
        changeHpPacket.Hp = StatInfo.Hp;
        Zone.BroadCast(CellPos,changeHpPacket);

        if (StatInfo.Hp <= 0)
        {
            OnDead(attacker);
        }
    }
    public virtual void OnDead(GameObject attacker)
    {
        if (Zone == null)
        {
            return;
        }

        S2C_Dead deadPacket = new S2C_Dead();
        deadPacket.ObjectId = Id;
        deadPacket.AttackerId = attacker.Id;
        Zone.BroadCast(CellPos, deadPacket);

        var zone = Zone;
        zone.LeaveZone(this);

        StatInfo.Hp = StatInfo.MaxHp;
        PosInfo.State = CreatureState.Idle;
        PosInfo.MoveDir = MoveDir.Down;

        zone.EnterZone(this, true);
    }
    public virtual GameObject GetOwner()
    {
        return this;
    }
}
