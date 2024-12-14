namespace CS_Server;

public class Area
{
    public int IndexY { get; set; }
    public int IndexX { get; set; }
    public HashSet<Player> Players { get; set; } = new HashSet<Player>();
    public Area(int indexY, int indexX)
    {
        IndexY = indexY;
        IndexX = indexX;
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
