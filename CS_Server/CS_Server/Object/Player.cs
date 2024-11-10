using Google.Protobuf;
using Google.Protobuf.Common;

namespace CS_Server;

public class Player
{
    private readonly ClientSession _session;
    public Zone? _zone;
    public PlayerInfo _playerInfo;

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

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }
}
