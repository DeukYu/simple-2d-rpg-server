using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using System.Collections.Concurrent;

namespace CS_Server;

public class PlayerManager
{
    public static PlayerManager Instance { get; } = new PlayerManager();
    private ConcurrentDictionary<long, Player> _players = new ConcurrentDictionary<long, Player>();
    private long _playerId = 0;  

    public Player? Add(ClientSession session)
    {
        PlayerInfo playerInfo = new PlayerInfo
        {
            PlayerId = Interlocked.Increment(ref _playerId),
            Name = "Player" + _playerId,
            PosInfo = new PositionInfo
            {
                State = CreatureState.Idle,
                MoveDir = MoveDir.Down,
                PosX = 0,
                PosY = 0,
            },
        };

        Player player = new Player(session, playerInfo);

        return _players.TryAdd(player._playerInfo.PlayerId, player) ? player : null;
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
