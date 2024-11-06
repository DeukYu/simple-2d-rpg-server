using System.Collections.Concurrent;

namespace CS_Server;

public class PlayerManager
{
    public static PlayerManager Instance { get; } = new PlayerManager();
    private ConcurrentDictionary<long, Player> _players = new ConcurrentDictionary<long, Player>();
    private int _playerId = 0;  // TODO : 임시적

    public Player? Add(ClientSession session)
    {
        Player player = new Player(session, Interlocked.Increment(ref _playerId));

        return _players.TryAdd(player._playerId, player) ? player : null;
    }

    public bool Remove(long playerId)
    {
        return _players.TryRemove(playerId, out _);
    }

    public Player? Find(long playerId)
    {
        return _players.TryGetValue(playerId, out var player) ? player : null;
    }
}
