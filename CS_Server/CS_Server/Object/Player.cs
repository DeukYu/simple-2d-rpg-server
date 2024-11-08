using Google.Protobuf;
using Google.Protobuf.Common;

namespace CS_Server;

public class Player
{
    private readonly ClientSession _session;
    private Zone? _zone;
    public TPlayer _playerInfo;

    public Player(ClientSession session, TPlayer tPlayer)
    {
        _session = session;
        _playerInfo = tPlayer;
        _zone = null;
    }

    public void EnterZone(Zone zone)
    {
        _zone = zone;
        zone.EnterZone(this);
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
