using Google.Protobuf.Enum;

namespace CS_Server;

public class Area
{
    public int IndexY { get; set; }
    public int IndexX { get; set; }
    public HashSet<Player> Players { get; set; } = new HashSet<Player>();
    public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();
    public HashSet<Projectile> Projectiles { get; set; } = new HashSet<Projectile>();
    private Dictionary<GameObjectType, HashSet<GameObject>> _gameObjects = new()
    {
        { GameObjectType.Player, new HashSet<GameObject>() },
        { GameObjectType.Monster, new HashSet<GameObject>() },
        { GameObjectType.Projectile, new HashSet<GameObject>() }
    };
    public Area(int indexY, int indexX)
    {
        IndexY = indexY;
        IndexX = indexX;
    }
    public void AddGameObject(GameObject gameObject)
    {
        var type = ObjectManager.GetObjectTypeById(gameObject.Id);
        if (_gameObjects.TryGetValue(type, out var gameObjects))
        {
            gameObjects.Add(gameObject);
        }
    }
    public HashSet<T> GetGameObjects<T>(GameObjectType type) where T : GameObject
    {
        return _gameObjects[type].Cast<T>().ToHashSet();
    }
    public void RemoveGameObject(GameObject gameObject)
    {
        var type = ObjectManager.GetObjectTypeById(gameObject.Id);
        if (_gameObjects.TryGetValue(type, out var gameObjects))
        {
            gameObjects.Remove(gameObject);
        }
    }
    public Player? FindPlayer(Func<Player, bool> condition)
    {
        return _gameObjects[GameObjectType.Player]
            .Cast<Player>()
            .FirstOrDefault(condition);
    }
    public List<Player> FindPlayers(Func<Player, bool> condition)
    {
        return _gameObjects[GameObjectType.Player]
            .Cast<Player>()
            .Where(condition)
            .ToList();
    }
}
