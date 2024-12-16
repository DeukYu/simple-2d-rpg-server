using Google.Protobuf.Enum;

namespace CS_Server;

public class Area
{
    public int IndexY { get; set; }
    public int IndexX { get; set; }
    public HashSet<Player> Players { get; set; } = new HashSet<Player>();
    public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();
    public HashSet<Projectile> Projectiles { get; set; } = new HashSet<Projectile>();
    public Area(int indexY, int indexX)
    {
        IndexY = indexY;
        IndexX = indexX;
    }
    public void Remove(GameObject gameObject)
    {
        var type = ObjectManager.GetObjectTypeById(gameObject.Id);

        if (type == GameObjectType.Player)
        {
            Players.Remove(gameObject as Player);
        }
        else if (type == GameObjectType.Monster)
        {
            Monsters.Remove(gameObject as Monster);
        }
        else if (type == GameObjectType.Projectile)
        {
            Projectiles.Remove(gameObject as Projectile);
        }
    }
    public Player FindPlayer(Func<Player, bool> condition)
    {
        foreach (Player player in Players)
        {
            if (condition.Invoke(player))
            {
                return player;
            }
        }
        return null;
    }
    public List<Player> FindPlayers(Func<Player, bool> condition)
    {
        var players = new List<Player>();
        foreach (Player player in Players)
        {
            if (condition.Invoke(player))
            {
                players.Add(player);
            }
        }
        return players;
    }
}
