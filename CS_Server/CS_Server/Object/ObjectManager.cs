using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class ObjectManager
{
    public static ObjectManager Instance { get; } = new ObjectManager();
    private ConcurrentDictionary<long, Player> _players = new ConcurrentDictionary<long, Player>();
    object _lock = new object();


    private long _playerId = 0; // TODO : 추후 DB 연동시, PlayerId 는 DB에서 불러올 예정
    int _counter = 0;

    public T Add<T>() where T : GameObject, new()
    {
        T gameObject = new T();
        gameObject.Id = GenerateId(gameObject.ObjectType);

        if (gameObject.ObjectType == GameObjectType.Player)
        {
            if (_players.TryAdd(gameObject.Id, gameObject as Player))
            {
                Log.Error($"Add Player : {gameObject.Id}");
                return gameObject;

            }
        }

        return gameObject;
    }

    long GenerateId(GameObjectType type)
    {
        lock (_lock)
        {
            return ((int)type << 24) | (_counter++);
        }
    }

    public static GameObjectType GetObjectTypeById(long id)
    {
        var type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public bool Remove(long objectId)
    {
        GameObjectType objectType = GetObjectTypeById(objectId);

        lock (_lock)
        {
            if (objectType == GameObjectType.Player)
            {
                return _players.TryRemove(objectId, out _);
            }
        }
        return false;
    }

    public Player? Find(long objectId)
    {
        GameObjectType objectType = GetObjectTypeById(objectId);

        if (objectType == GameObjectType.Player)
        {
            return _players.TryGetValue(objectId, out var player) ? player : null;
        }
        return null;
    }
}
