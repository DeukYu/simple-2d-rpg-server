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
    private bool IsWithinVision(GameObject obj, Vector2Int ownerPos)
    {
        var destPos = obj.CellPos - ownerPos;
        
        return Math.Abs(destPos.x) <= Zone.VisionCells && Math.Abs(destPos.y) <= Zone.VisionCells;
    }

    private void AddObjectsToSet(HashSet<GameObject> objects, IEnumerable<GameObject> source, Vector2Int ownerPos)
    {
        foreach (var obj in source)
        {
            if (IsWithinVision(obj, ownerPos))
            {
                objects.Add(obj);
            }
        }
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

        foreach(var area in areas)
        {
            AddObjectsToSet(objects, area.Players, cellPos);
            AddObjectsToSet(objects, area.Monsters, cellPos);
            AddObjectsToSet(objects, area.Projectiles, cellPos);
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

        // 새로 추가된 객체 처리 (Spawn)
        HandleSpawn(currentObjects);

        // 사라진 객체 처리 (Despawn)
        HandleDespawn(currentObjects);

        // 현재 객체 목록을 이전 객체 목록으로 갱신
        PreviousObjects = currentObjects;

        // 일정 시간 후 업데이트 스케줄링
        Owner.Zone.ScheduleJobAfterDelay(500, Update);
    }

    private void HandleSpawn(HashSet<GameObject> currentObjects)
    {
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
    }
    private void HandleDespawn(HashSet<GameObject> currentObjects)
    {
        var removeObjects = PreviousObjects.Except(currentObjects).ToList();
        if (removeObjects.Count > 0)
        {
            S2C_Despawn despawn = new S2C_Despawn();
            foreach (var obj in removeObjects)
            {
                despawn.ObjectIds.Add(obj.Id);
            }
            Owner.Session.Send(despawn);
        }
    }
}
