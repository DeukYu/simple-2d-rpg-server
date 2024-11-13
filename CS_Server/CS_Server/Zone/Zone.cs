using Google.Protobuf;
using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Net.Http.Headers;

namespace CS_Server;

public class Zone
{
    public long ZoneId { get; set; }
    object _lock = new object();

    private readonly Dictionary<GameObjectType, Action<GameObject>> _addToZoneActions;
    private readonly Dictionary<GameObjectType, Action<GameObject>> _removeToZoneActions;

    Dictionary<long, Player> _players = new Dictionary<long, Player>(); // TODO : JobQueue 형식으로 변경시, ConcurrentDictionary 으로 변경하여, 락프리(?)로 변경한다.
    Dictionary<long, Monster> _monsters = new Dictionary<long, Monster>();
    Dictionary<long, Projectile> _projectiles = new Dictionary<long, Projectile>();

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

    private void AddPlayerToZone(GameObject gameObject)
    {
        var player = gameObject as Player;

        _players.Add(gameObject.Info.ObjectId, player);
        player._zone = this;

        {
            S2C_EnterGame pkt = new S2C_EnterGame
            {
                Result = (int)ErrorType.Success,
                ObjectInfo = gameObject.Info
            };

            player.Session.Send(pkt);

            var filteredPlayers = _players.Values.Where(p => gameObject != p).Select(p => p.Info).ToList();
            if (filteredPlayers.Count > 0)
            {
                S2C_Spawn spawn = new S2C_Spawn();
                spawn.Objects.AddRange(filteredPlayers);
                player.Session.Send(spawn);
            }
        }
    }

    private void AddMonsterToZone(GameObject gameObject)
    {
        var monster = gameObject as Monster;
        _monsters.Add(gameObject.Info.ObjectId, monster);
        monster._zone = this;
    }

    private void AddProjectileToZone(GameObject gameObject)
    {
        var projectile = gameObject as Projectile;
        _projectiles.Add(gameObject.Info.ObjectId, projectile);
        projectile._zone = this;
    }

    private void RemovePlayerFromZone(GameObject gameObject)
    {
        if (_players.TryGetValue(gameObject.Id, out var player) == false)
        {
            Log.Error("LeaveZone player is null");
            return;
        }

        _players.Remove(gameObject.Id);
        player._zone = null;
        Map.ApplyLeave(player);
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

        monster._zone = null;
        Map.ApplyLeave(monster);
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
    }

    public void Update()
    {
        lock (_lock)
        {
            foreach (var projectile in _projectiles.Values)
            {
                projectile.Update();
            }
        }
    }

    public void EnterZone(GameObject gameObject)
    {
        if (gameObject == null)
        {
            Log.Error("EnterZone player is null");
            return;
        }

        lock (_lock)
        {
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

        lock (_lock)
        {
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
    }
    public void HandleMove(Player player, C2S_Move packet)
    {
        if (player == null)
        {
            Log.Error("HandleMove player is null");
            return;
        }

        lock (_lock)
        {
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

            playerInfo.PosInfo.State = movePosInfo.State;
            playerInfo.PosInfo.MoveDir = movePosInfo.MoveDir;
            Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));


            S2C_Move res = new S2C_Move
            {
                ObjectId = player.Info.ObjectId,
                PosInfo = packet.PosInfo,
            };

            foreach (var p in _players.Values)
            {
                p.Session.Send(res);
            }
        }
    }

    public void HandleSkill(Player player, C2S_Skill packet)
    {
        if (player == null)
        {
            Log.Error("HandleSkill player is null");
            return;
        }

        lock (_lock)
        {
            ObjectInfo info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
            {
                Log.Error("HandleSkill player is not idle");
                return;
            }



            info.PosInfo.State = CreatureState.Skill;

            S2C_Skill res = new S2C_Skill
            {
                ObjectId = player.Info.ObjectId,
                SkillInfo = new SkillInfo
                {
                    SkillId = 1,
                }
            };

            foreach (var p in _players.Values)
                p.Session.Send(res);

            //BroadCast(res);

            if(DataManager.SkillDict.TryGetValue(packet.SkillInfo.SkillId, out var skillData) == false)
            {
                Log.Error($"HandleSkill skillData is null. SkillId{packet.SkillInfo.SkillId}");
                return;
            }

            switch(skillData.SkillType)
            {
                case SkillType.SkillAuto:
                    {
                        var skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                        var target = Map.Find(skillPos);
                        if (target != null)
                        {
                            Log.Info("GameObject Hit");
                        }
                    }
                    break;
                case SkillType.SkillProjectile:
                    {
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
                        EnterZone(arrow);
                    }
                    break;
            }
        }
    }

    public void BroadCast(IMessage packet)
    {
        lock (_lock)
        {
            foreach (var player in _players.Values)
            {
                player.Session.Send(packet);
            }
        }
    }
}
