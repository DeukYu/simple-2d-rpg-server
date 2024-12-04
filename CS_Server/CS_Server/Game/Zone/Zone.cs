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

public class Zone : JobSerializer
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
        monster.CellPos = new Vector2Int(10, 10);
        Push(EnterZone, monster);
    }

    public void Update()
    {
        foreach (var projectile in _projectiles.Values)
        {
            projectile.Update();
        }

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

    private bool IsValidMove(Player player, PositionInfo movePosInfo)
    {
        var currentPos = new Vector2Int(player.PosInfo.PosX, player.PosInfo.PosY);
        var destPos = new Vector2Int(movePosInfo.PosX, movePosInfo.PosY);

        if (currentPos == destPos)
        {
            return true;
        }

        return Map.CanGo(destPos);
    }

    private void UpdatePlayerPosition(Player player, PositionInfo movePosInfo)
    {
        ObjectInfo playerInfo = player.Info;
        playerInfo.PosInfo.State = movePosInfo.State;
        playerInfo.PosInfo.MoveDir = movePosInfo.MoveDir;
        Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));
    }

    public void HandleMove(Player player, C2S_Move packet)
    {
        if (player == null)
        {
            Log.Error("HandleMove player is null");
            return;
        }

        if (IsValidMove(player, packet.PosInfo) == false)
        {
            return;
        }

        PositionInfo movePosInfo = packet.PosInfo;
        ObjectInfo playerInfo = player.Info;

        // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
        if (movePosInfo.PosX != playerInfo.PosInfo.PosX || movePosInfo.PosY != playerInfo.PosInfo.PosY)
        {
            if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
            {
                return;
            }
        }

        UpdatePlayerPosition(player, movePosInfo);

        S2C_Move res = new S2C_Move
        {
            ObjectId = player.Info.ObjectId,
            PosInfo = packet.PosInfo,
        };
        BroadCast(res);
    }

    public void HandleSkill(Player player, C2S_Skill packet)
    {
        if (player == null)
        {
            Log.Error("HandleSkill player is null");
            return;
        }

        if (CanUseSkill(player) == false)
        {
            return;
        }

        var skillData = GetSkillData(packet.SkillInfo.SkillId);
        if (skillData == null)
        {
            Log.Error("HandleSkill skillData is null");
            return;
        }

        switch (skillData.SkillType)
        {
            case SkillType.Auto:
                HandleAutoSkill(player, skillData);
                break;

            case SkillType.Projectile:
                HandleProjectileSkill(player, skillData);
                break;

            default:
                Log.Error("HandleSkill invalid skill type");
                return;
        }

        S2C_Skill res = new S2C_Skill
        {
            ObjectId = player.Info.ObjectId,
            SkillInfo = new SkillInfo
            {
                SkillId = 1,
            }
        };
        BroadCast(res);
    }

    private bool CanUseSkill(Player player)
    {
        ObjectInfo info = player.Info;
        if (info.PosInfo.State != CreatureState.Idle)
        {
            Log.Error("HandleSkill player is not idle");
            return false;
        }

        info.PosInfo.State = CreatureState.Skill;

        return true;
    }

    private SkillData? GetSkillData(int skillId)
    {
        if (DataManager.SkillDict.TryGetValue(skillId, out var skillData) == false)
        {
            Log.Error($"GetSkillData skillData is null. SkillId{skillId}");
            return null;
        }
        return skillData;
    }

    private void HandleAutoSkill(Player player, SkillData skillData)
    {
        var skillPos = player.GetFrontCellPos(player.PosInfo.MoveDir);
        var target = Map.Find(skillPos);
        if (target != null)
        {
            target.OnDamaged(player, skillData.Damage + player.StatInfo.Attack);
        }
    }

    private void HandleProjectileSkill(Player player, SkillData skillData)
    {
        if (DataManager.ProjectileInfoDict.TryGetValue(skillData.ProjectileId, out var projectileInfo) == false)
        {
            Log.Error($"HandleSkill projectileInfo is null. ProjectileId{skillData.ProjectileId}");
            return;
        }
        var arrow = ObjectManager.Instance.Add<Arrow>();
        if (arrow == null)
        {
            Log.Error("HandleSkill arrow is null");
            return;
        }

        arrow.Owner = player;
        arrow.SkillData = skillData;
        arrow.PosInfo.State = CreatureState.Move;
        arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
        arrow.PosInfo.PosX = player.PosInfo.PosX;
        arrow.PosInfo.PosY = player.PosInfo.PosY;
        arrow.Speed = projectileInfo.Speed;
        Push(EnterZone, arrow);
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
