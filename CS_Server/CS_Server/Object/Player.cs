using Google.Protobuf;
using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using System.Net.Http.Headers;

namespace CS_Server;

public class Player
{
    private readonly ClientSession _session;
    public Zone? _zone;
    public PlayerInfo _playerInfo;

    public Vector2Int CellPos
    {
        get
        {
            return new Vector2Int(_playerInfo.PosInfo.PosX, _playerInfo.PosInfo.PosY);
        }
        set
        {
            _playerInfo.PosInfo.PosX = value.x;
            _playerInfo.PosInfo.PosY = value.y;
        }
    }

    public Vector2Int GetFrontCellPos(MoveDir dir)
    {
        Vector2Int cellPos = CellPos;

        switch(dir)
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

    public Player(ClientSession session, PlayerInfo tPlayer)
    {
        _session = session;
        _playerInfo = tPlayer;
        _zone = null;
    }

    public void EnterZone(Zone zone)
    {
        _zone = zone;
    }

    public void LeaveZone()
    {
        _zone = null;
    }

    // TODO : 추후 JobTimer로 될 예정이나, 현재는 임시로 Send를 보낸다.
    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }
}
