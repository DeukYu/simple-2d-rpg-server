namespace CS_Server;

public class Player
{
    public readonly ClientSession _session;
    public readonly long _playerId;

    public Player(ClientSession session, long playerId)
    {
        _session = session;
        _playerId = playerId;
    }
}
