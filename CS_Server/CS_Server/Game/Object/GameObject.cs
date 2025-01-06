using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;

namespace CS_Server;

public class GameObject
{
    public Zone Zone { get; set; }
    public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
    public ObjectInfo Info { get; set; } = new ObjectInfo();
    public PositionInfo PosInfo { get; private set; } = new PositionInfo();
    public StatInfo StatInfo { get; private set; } = new StatInfo();
    public virtual int TotalAttack => StatInfo.Attack;
    public virtual int TotalDefense => 0;
    public virtual GameObject GetOwner() => this;
    public float Speed
    {
        get => StatInfo.Speed;
        set => StatInfo.Speed = value;
    }

    public MoveDir Dir
    {
        get => PosInfo.MoveDir;
        set => PosInfo.MoveDir = value;
    }

    public CreatureState State
    {
        get => PosInfo.State;
        set => PosInfo.State = value;
    }
    public int Hp
    {
        get => StatInfo.Hp;
        set => StatInfo.Hp = Math.Clamp(value, 0, StatInfo.MaxHp);
    }

    public int Id
    {
        get => Info.ObjectId;
        set => Info.ObjectId = value;
    }

    public GameObject()
    {
        Info.PosInfo = PosInfo;
        Info.StatInfo = StatInfo;
    }

    public virtual void Update() { }

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

    private void ApplyDamage(int damage)
    {
        StatInfo.Hp -= damage;
        StatInfo.Hp = Math.Max(StatInfo.Hp, 0);
    }

    public virtual void OnDamaged(GameObject attacker, int damage)
    {
        if (Zone == null)
            return;

        damage = Math.Max(damage - TotalDefense, 0);

        ApplyDamage(damage);

        Zone.BroadCast(CellPos, new S2C_ChangeHp
        {
            ObjectId = Id,
            Hp = StatInfo.Hp
        });

        if (StatInfo.Hp <= 0)
            OnDead(attacker);
    }

    private void OnRivive(Zone zone)
    {
        zone.LeaveZone(this);
        StatInfo.Hp = StatInfo.MaxHp;
        PosInfo.State = CreatureState.Idle;
        PosInfo.MoveDir = MoveDir.Down;
        zone.EnterZone(this, true);
    }
    public virtual void OnDead(GameObject attacker)
    {
        if (Zone == null)
            return;

        var zone = Zone;
        zone.BroadCast(CellPos, new S2C_Dead
        {
            ObjectId = Id,
            AttackerId = attacker.Id
        });

        OnRivive(zone);
    }
}
