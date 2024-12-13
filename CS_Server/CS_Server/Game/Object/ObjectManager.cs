using Google.Protobuf.Enum;
using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class ObjectManager
{
    public static ObjectManager Instance { get; } = new ObjectManager();
    private ConcurrentDictionary<int, Player> _players = new ConcurrentDictionary<int, Player>();

    private int _counter = 0;

    int GenerateIdForType(GameObjectType type)
    {
        int uniqueCounter = Interlocked.Increment(ref _counter);
        return ((int)type << 24) | uniqueCounter;
    }

    public static GameObjectType GetObjectTypeById(int objectId)
    {
        var type = (objectId >> 24) & 0x7F;
        return (GameObjectType)type;
    }
    public T Add<T>() where T : GameObject, new()
    {
        T gameObject = new T();

        gameObject.Id = GenerateIdForType(gameObject.ObjectType);

        if (gameObject.ObjectType == GameObjectType.Player)
        {
            if (_players.TryAdd(gameObject.Id, gameObject as Player) == false)
            {
                Log.Error($"Add Player : {gameObject.Id}");
                return null;
            }
        }

        // TODO : 추후 다른 타입 처리가 필요한 경우 추가

        return gameObject;
    }

    public bool Remove(int objectId)
    {
        GameObjectType objectType = GetObjectTypeById(objectId);

        return objectType switch
        {
            GameObjectType.Player => _players.TryRemove(objectId, out _),
            _ => false
        };
    }

    public Player? Find(int objectId)
    {
        GameObjectType objectType = GetObjectTypeById(objectId);

        return objectType switch
        {
            GameObjectType.Player => _players.TryGetValue(objectId, out var player) ? player : null,
            _ => null
        };
    }
}
