using Google.Protobuf.Common;
using Google.Protobuf.Enum;

namespace CS_Server;

public class GameObject
{
    public Zone? _zone;
    public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
    public ObjectInfo Info { get; set; } = new ObjectInfo();
    public PositionInfo PosInfo { get; private set; } = new PositionInfo();

    public long Id
    {
        get { return Info.ObjectId; }
        set { Info.ObjectId = value; }
    }

    public GameObject()
    {
        Info.PosInfo = PosInfo;
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
}
