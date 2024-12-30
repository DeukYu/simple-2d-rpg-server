using Google.Protobuf;
using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public partial class Zone : JobSerializer
{
    public const int VisionCells = 5;
    public long ZoneId { get; set; }

    private readonly Dictionary<GameObjectType, Action<GameObject>> _addToZoneActions;
    private readonly Dictionary<GameObjectType, Action<GameObject>> _removeToZoneActions;

    private ConcurrentDictionary<int, Player> _players = new ConcurrentDictionary<int, Player>();
    private ConcurrentDictionary<int, Monster> _monsters = new ConcurrentDictionary<int, Monster>();
    private ConcurrentDictionary<int, Projectile> _projectiles = new ConcurrentDictionary<int, Projectile>();

    // Area
    public Area[,] Areas { get; private set; }
    public int AreaCells { get; private set; }
    // Map
    public Map Map { get; private set; } = new Map();

    public Zone()
    {
        _addToZoneActions = new Dictionary<GameObjectType, Action<GameObject>>()
                        {
                            { GameObjectType.Player, AddPlayerToZone },
                            { GameObjectType.Monster, AddMonsterToZone },
                            { GameObjectType.Projectile, AddProjectileToZone }
                        };

        _removeToZoneActions = new Dictionary<GameObjectType, Action<GameObject>>()
                        {
                            { GameObjectType.Player, RemovePlayerFromZone },
                            { GameObjectType.Monster, RemoveMonsterFromZone },
                            { GameObjectType.Projectile, RemoveProjectileFromZone }
                        };
    }

    public Area GetArea(Vector2Int cellPos)
    {
        int x = (cellPos.x - Map.Bounds.MinX) / AreaCells;
        int y = (Map.Bounds.MaxY - cellPos.y) / AreaCells;

        return GetArea(y, x);
    }

    public Area GetArea(int indexY, int indexX)
    {
        if (indexX < 0 || indexX >= Areas.GetLength(1))
            return null;

        if (indexY < 0 || indexY >= Areas.GetLength(0))
            return null;

        return Areas[indexY, indexX];
    }

    public void Init(int mapId, int areaCells)
    {
        Map.LoadMap(mapId, "../../../../Common/MapData");

        // Area
        AreaCells = areaCells;
        int countY = (Map.Bounds.SizeY + areaCells - 1) / areaCells;
        int countX = (Map.Bounds.SizeX + areaCells - 1) / areaCells;
        Areas = new Area[countY, countX];
        for (int y = 0; y < countY; y++)
        {
            for (int x = 0; x < countX; x++)
            {
                Areas[y, x] = new Area(y, x);
            }
        }

        // TODO : Monster
        for (int i = 0; i < 10; ++i)
        {
            var monster = ObjectManager.Instance.Add<Monster>();
            monster.Init(1);
            monster.CellPos = new Vector2Int(10, 10);
            EnterZone(monster, randomPos: true);
        }
    }

    public void Update()
    {
        ProcessJobs();
    }

    public void AddGameObjectToZone(GameObject gameObject)
    {
        if (_addToZoneActions.TryGetValue(gameObject.ObjectType, out var addAction))
        {
            addAction(gameObject);
        }
        else
        {
            Log.Error($"Unsupported GameObjectType: {gameObject.ObjectType}");
        }
    }

    public void RemoveGameObjectFromZone(GameObject gameObject)
    {
        if (_removeToZoneActions.TryGetValue(gameObject.ObjectType, out var removeAction))
        {
            removeAction(gameObject);
        }
        else
        {
            Log.Error($"Unsupported GameObjectType: {gameObject.ObjectType}");
        }
    }

    private List<ObjectInfo> CollectSpawnableObjects(GameObject excludeObject)
    {
        var allObjects = new List<ObjectInfo>();
        allObjects.AddRange(_players.Values.Where(p => excludeObject != p).Select(p => p.Info));
        allObjects.AddRange(_monsters.Values.Select(m => m.Info));
        allObjects.AddRange(_projectiles.Values.Select(p => p.Info));
        return allObjects;
    }

    private void AddPlayerToZone(GameObject gameObject)
    {
        if (gameObject is not Player player)
        {
            Log.Error("AddPlayerToZone: Invalid GameObject type");
            return;
        }

        if (_players.TryAdd(gameObject.Info.ObjectId, player) == false)
        {
            Log.Error($"AddPlayerToZone player {gameObject.Info.ObjectId} is already exist");
            return;
        }
        player.Zone = this;

        player.RefreshAdditionalStat();

        Map.ApplyMove(player, player.CellPos);

        GetArea(player.CellPos).Players.Add(player);

        // 플레이어 입장 처리
        var allObjects = CollectSpawnableObjects(gameObject);
        player.OnEnterGame(allObjects);
    }

    private void AddMonsterToZone(GameObject gameObject)
    {
        var monster = gameObject as Monster;
        if (monster == null)
        {
            Log.Error("AddMonsterToZone monster is null");
            return;
        }

        _monsters.TryAdd(gameObject.Info.ObjectId, monster);
        monster.Zone = this;
        Map.ApplyMove(monster, monster.CellPos);
        GetArea(monster.CellPos).Monsters.Add(monster);

        monster.Update();
    }

    private void AddProjectileToZone(GameObject gameObject)
    {
        var projectile = gameObject as Projectile;
        if (projectile == null)
        {
            Log.Error("AddProjectileToZone projectile is null");
            return;
        }
        _projectiles.TryAdd(gameObject.Info.ObjectId, projectile);
        projectile.Zone = this;
        GetArea(projectile.CellPos).Projectiles.Add(projectile);

        projectile.Update();
    }

    private void RemovePlayerFromZone(GameObject gameObject)
    {
        if (_players.TryGetValue(gameObject.Id, out var player) == false)
        {
            Log.Error("LeaveZone player is null");
            return;
        }

        player.OnLeaveGame();
        _players.Remove(gameObject.Id, out _);
        Map.ApplyLeave(player);
        player.Zone = null;
        {
            S2C_LeaveGame leave = new S2C_LeaveGame();
            player.Session.Send(leave);
        }
    }

    private void RemoveMonsterFromZone(GameObject gameObject)
    {
        if (_monsters.TryGetValue(gameObject.Id, out var monster) == false)
        {
            Log.Error("LeaveZone monster is null");
            return;
        }

        Map.ApplyLeave(monster);
        monster.Zone = null;
    }

    private void RemoveProjectileFromZone(GameObject gameObject)
    {
        if (_projectiles.TryGetValue(gameObject.Id, out var projectile) == false)
        {
            Log.Error("LeaveZone projectile is null");
            return;
        }

        Map.ApplyLeave(projectile);

        projectile.Zone = null;
    }

    Random _rand = new Random();
    public void EnterZone(GameObject gameObject, bool randomPos = false)
    {
        if (gameObject == null)
        {
            Log.Error("EnterZone player is null");
            return;
        }

        // TODO : respawn 위치 
        if (randomPos)
        {
            Vector2Int respawnPos;
            while (true)
            {
                respawnPos.x = _rand.Next(Map.Bounds.MinX, Map.Bounds.MaxX);
                respawnPos.y = _rand.Next(Map.Bounds.MinY, Map.Bounds.MaxY);
                if (Map.Find(respawnPos) == null)
                {
                    break;
                }

                gameObject.CellPos = respawnPos;
                break;
            }
        }

        AddGameObjectToZone(gameObject);

        {
            S2C_Spawn spawn = new S2C_Spawn
            {
                Objects = { gameObject.Info }
            };

            BroadCast(gameObject.CellPos, spawn, p => p.Id != gameObject.Id);
        }
    }

    public void LeaveZone(GameObject gameObject)
    {
        if (gameObject == null)
        {
            Log.Error("EnterZone player is null");
            return;
        }

        var type = gameObject switch
        {
            Player p => p.ObjectType,
            Monster m => m.ObjectType,
            Projectile p => p.ObjectType,
            _ => GameObjectType.None
        };

        RemoveGameObjectFromZone(gameObject);

        {
            S2C_Despawn despawn = new S2C_Despawn
            {
                ObjectIds = { gameObject.Id }
            };
            BroadCast(gameObject.CellPos, despawn);
        }
    }

    public Player FindClosedPlayer(Vector2Int cellPos, int range)
    {
        var players = GetAdjacentPlayers(cellPos, range);

        players.Sort((lhs, rhs) =>
        {
            int lhsDist = (lhs.CellPos - cellPos).cellDistance;
            int rhsDist = (rhs.CellPos - cellPos).cellDistance;
            return lhsDist - rhsDist;
        });

        foreach (var player in players)
        {
            var paths = Map.FindPath(cellPos, player.CellPos, checkObjects: true);
            if (paths.Count < 2 || paths.Count > range)
                continue;
            return player;
        }
        return null;
    }

    public void BroadCast(Vector2Int pos, IMessage packet, Func<Player, bool>? filter = null)
    {
        var areas = GetAdjacentArea(pos);
        foreach (var player in areas.SelectMany(a => a.Players))
        {
            int dx = player.CellPos.x - pos.x;
            int dy = player.CellPos.y - pos.y;

            if (Math.Abs(dx) > Zone.VisionCells)
            {
                continue;
            }
            if (Math.Abs(dy) > Zone.VisionCells)
            {
                continue;
            }

            if (filter == null || filter(player))
                player.Session.Send(packet);
        }
    }

    public List<Player> GetAdjacentPlayers(Vector2Int cellPos, int range)
    {
        var areas = GetAdjacentArea(cellPos, range);

        return areas.SelectMany(a => a.Players).ToList();
    }

    public List<Area> GetAdjacentArea(Vector2Int cellPos, int range = VisionCells)
    {
        var areas = new HashSet<Area>();

        int maxY = cellPos.y + range;
        int minY = cellPos.y - range;
        int maxX = cellPos.x + range;
        int minX = cellPos.x - range;

        // 좌측 상단
        var leftTop = new Vector2Int(minX, maxY);

        int minIndexY = (Map.Bounds.MaxY - leftTop.y) / AreaCells;
        int minIndexX = (leftTop.x - Map.Bounds.MinX) / AreaCells;

        // 우축 하단
        var rightBottom = new Vector2Int(maxX, minY);
        int maxIndexY = (Map.Bounds.MaxY - rightBottom.y) / AreaCells;
        int maxIndexX = (rightBottom.x - Map.Bounds.MinX) / AreaCells;

        for (int x = minIndexX; x <= maxIndexX; x++)
        {
            for (int y = minIndexY; y <= maxIndexY; y++)
            {
                var area = GetArea(y, x);
                if (area == null)
                    continue;

                areas.Add(area);
            }
        }

        int[] delta = new int[2] { -range, +range };

        foreach (int dy in delta)
        {
            foreach (int dx in delta)
            {
                int y = cellPos.y + dy;
                int x = cellPos.x + dx;
                var area = GetArea(new Vector2Int(x, y));
                if (area == null)
                    continue;
                areas.Add(area);
            }
        }

        return areas.ToList();
    }
}
