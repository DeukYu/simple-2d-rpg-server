using Google.Protobuf.Enum;
using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class ObjectManager
{
    public static ObjectManager Instance { get; } = new ObjectManager();
    private ConcurrentDictionary<int, Player> _players = new ConcurrentDictionary<int, Player>();

    private int _counter = 0;
    int GenerateId(GameObjectType type)
    {
        int uniqueCounter = Interlocked.Increment(ref _counter);
        return ((int)type << 24) | uniqueCounter;
    }

    public T Add<T>() where T : GameObject, new()
    {
        T gameObject = new T();

        gameObject.Id = GenerateId(gameObject.ObjectType);

        if (gameObject.ObjectType == GameObjectType.Player)
        {
            if (_players.TryAdd(gameObject.Id, gameObject as Player) == false)
            {
                Log.Error($"Add Player : {gameObject.Id}");
                return null;
            }
        }

        return gameObject;
    }

    public static GameObjectType GetObjectTypeById(int objectId)
    {
        var type = (objectId >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public bool Remove(int objectId)
    {
        GameObjectType objectType = GetObjectTypeById(objectId);

        if (objectType == GameObjectType.Player)
        {
            return _players.TryRemove(objectId, out _);
        }
        return false;
    }

    public Player? Find(int objectId)
    {
        GameObjectType objectType = GetObjectTypeById(objectId);

        if (objectType == GameObjectType.Player)
        {
            return _players.TryGetValue(objectId, out var player) ? player : null;
        }
        return null;
    }
}
