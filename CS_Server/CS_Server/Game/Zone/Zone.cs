using Google.Protobuf;
using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ServerCore;
using Shared;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace CS_Server;

public partial class Zone : JobSerializer
{
    public long ZoneId { get; set; }

    private readonly Dictionary<GameObjectType, Action<GameObject>> _addToZoneActions;
    private readonly Dictionary<GameObjectType, Action<GameObject>> _removeToZoneActions;

    private ConcurrentDictionary<int, Player> _players = new ConcurrentDictionary<int, Player>();
    private ConcurrentDictionary<int, Monster> _monsters = new ConcurrentDictionary<int, Monster>();
    private ConcurrentDictionary<int, Projectile> _projectiles = new ConcurrentDictionary<int, Projectile>();

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
        player._zone = this;

        player.RefreshAdditionalStat();

        Map.ApplyMove(player, player.CellPos);

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
        monster._zone = this;
        Map.ApplyMove(monster, monster.CellPos);
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
        projectile._zone = this;

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
        player._zone = null;
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
        monster._zone = null;
    }

    private void RemoveProjectileFromZone(GameObject gameObject)
    {
        if (_projectiles.TryGetValue(gameObject.Id, out var projectile) == false)
        {
            Log.Error("LeaveZone projectile is null");
            return;
        }
        projectile._zone = null;
    }

    public Map Map { get; private set; } = new Map();

    public void Init(int mapId)
    {
        // TODO : 데이터 Sheet 들어가면 수정
        Map.LoadMap(mapId, "../../../../Common/MapData");

        // Monster
        var monster = ObjectManager.Instance.Add<Monster>();
        monster.Init(1);
        monster.CellPos = new Vector2Int(10, 10);
        Push(EnterZone, monster);
    }

    public void Update()
    {
        foreach (var monster in _monsters.Values)
        {
            monster.Update();
        }

        Flush();
    }

    public void EnterZone(GameObject gameObject)
    {
        if (gameObject == null)
        {
            Log.Error("EnterZone player is null");
            return;
        }

        AddGameObjectToZone(gameObject);

        {
            S2C_Spawn spawn = new S2C_Spawn
            {
                Objects = { gameObject.Info }
            };

            foreach (var p in _players.Values)
            {
                if (p.Id != gameObject.Id)
                    p.Session.Send(spawn);
            }
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
            foreach (var p in _players.Values)
            {
                if (p.Id != gameObject.Id)
                    p.Session.Send(despawn);
            }
        }
    }

    public Player? FindPlayer(Func<GameObject, bool> condition)
    {
        foreach (var player in _players.Values)
        {
            if (condition(player))
                return player;
        }
        return null;
    }

    public void BroadCast(IMessage packet, Func<Player, bool>? filter = null)
    {
        foreach (var player in _players.Values)
        {
            if (filter == null || filter(player))
                player.Session.Send(packet);
        }
    }
}
