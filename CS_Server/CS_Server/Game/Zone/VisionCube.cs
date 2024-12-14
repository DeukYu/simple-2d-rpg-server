namespace CS_Server.Game;

public class VisionCube
{
    public Player Owner { get; private set; }
    public HashSet<GameObject> PreviousObjects { get; private set; } = new HashSet<GameObject>();
    public VisionCube(Player owner)
    {
        Owner = owner;
    }
    public HashSet<GameObject> GatherObjects()
    {
        if (Owner == null || Owner.Zone == null)
        {
            return null;
        }

        HashSet<GameObject> objects = new HashSet<GameObject>();

        var cellPos = Owner.CellPos;
        List<Area> areas = Owner.Zone.GetAdjacentArea(cellPos);

        return objects;
    }
}
