using Google.Protobuf.Common;
using Google.Protobuf.Protocol;

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

        foreach (var area in areas)
        {
            foreach (var player in area.Players)
            {
                int dx = player.CellPos.x - cellPos.x;
                int dy = player.CellPos.y - cellPos.y;

                if (Math.Abs(dx) > Zone.VisionCells)
                {
                    continue;
                }
                if (Math.Abs(dy) > Zone.VisionCells)
                {
                    continue;
                }

                objects.Add(player);
            }

            foreach (var monster in area.Monsters)
            {
                int dx = monster.CellPos.x - cellPos.x;
                int dy = monster.CellPos.y - cellPos.y;

                if (Math.Abs(dx) > Zone.VisionCells)
                {
                    continue;
                }
                if (Math.Abs(dy) > Zone.VisionCells)
                {
                    continue;
                }

                objects.Add(monster);
            }

            foreach (var projectile in area.Projectiles)
            {
                int dx = projectile.CellPos.x - cellPos.x;
                int dy = projectile.CellPos.y - cellPos.y;

                if (Math.Abs(dx) > Zone.VisionCells)
                {
                    continue;
                }
                if (Math.Abs(dy) > Zone.VisionCells)
                {
                    continue;
                }

                objects.Add(projectile);
            }
        }

        return objects;
    }
    public void Update()
    {
        if (Owner == null || Owner.Zone == null)
        {
            return;
        }

        var currentObjects = GatherObjects();

        // 기존에 없었는데, 새로 생긴 애들 Spawn 처리
        var addObjects = currentObjects.Except(PreviousObjects).ToList();
        if (addObjects.Count > 0)
        {
            S2C_Spawn spawn = new S2C_Spawn();
            foreach (var obj in addObjects)
            {
                var objectInfo = new ObjectInfo();
                objectInfo.MergeFrom(obj.Info);
                spawn.Objects.Add(objectInfo);
            }
            Owner.Session.Send(spawn);
        }
        // 기존에 있었는데, 사라진 애들 Despawn 처리
        var removeObjects = PreviousObjects.Except(currentObjects).ToList();
        if(removeObjects.Count > 0)
        {
            S2C_Despawn despawn = new S2C_Despawn();
            foreach (var obj in removeObjects)
            {
                despawn.ObjectIds.Add(obj.Id);
            }
            Owner.Session.Send(despawn);
        }

        PreviousObjects = currentObjects;

        Owner.Zone.ScheduleJobAfterDelay(500, Update);
    }
}
