using Google.Protobuf;
using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class Zone
{
    public long ZoneId { get; set; }
    object _lock = new object();

    Dictionary<long, Player> _players = new Dictionary<long, Player>(); // TODO : JobQueue 형식으로 변경시, ConcurrentDictionary 으로 변경하여, 락프리(?)로 변경한다.

    Map _map = new Map();

    public void Init(int mapId)
    {
        // TODO : 데이터 Sheet 들어가면 수정
        _map.LoadMap(mapId, "../../../../Common/MapData");
    }

    public void EnterZone(Player player)
    {
        if (player == null)
        {
            Log.Error("EnterZone player is null");
            return;
        }

        lock (_lock)
        {
            _players.Add(player._playerInfo.PlayerId, player);
            player.EnterZone(this);

            {
                S2C_EnterGame pkt = new S2C_EnterGame
                {
                    Result = (int)ErrorType.Success,
                    PlayerInfo = player._playerInfo
                };

                player.Send(pkt);

                var filteredPlayers = _players.Values.Where(p => player != p).Select(p => p._playerInfo).ToList();
                if (filteredPlayers.Count > 0)
                {
                    S2C_Spawn spawn = new S2C_Spawn();
                    spawn.Players.AddRange(filteredPlayers);
                    player.Send(spawn);
                }
            }

            {
                S2C_Spawn spawn = new S2C_Spawn
                {
                    Players = { player._playerInfo }
                };

                foreach (var p in _players.Values)
                {
                    if (player != p)
                        p.Send(spawn);
                }
            }
        }
    }

    public void LeaveZone(long playerId)
    {
        lock (_lock)
        {
            if (_players.TryGetValue(playerId, out var player) == false)
            {
                Log.Error("LeaveZone player is null");
                return;
            }

            _players.Remove(playerId);
            player.LeaveZone();
            {
                S2C_LeaveGame leave = new S2C_LeaveGame();
                player.Send(leave);
            }

            {
                S2C_Despawn despawn = new S2C_Despawn
                {
                    PlayerIds = { playerId }
                };
                foreach (var p in _players.Values)
                {
                    if (player != p)
                        p.Send(despawn);
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
            PlayerInfo playerInfo = player._playerInfo;

            // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
            if(movePosInfo.PosX != playerInfo.PosInfo.PosX || movePosInfo.PosY != playerInfo.PosInfo.PosY)
            {
                if(_map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                {
                    return;
                }
            }

            playerInfo.PosInfo.State = movePosInfo.State;
            playerInfo.PosInfo.MoveDir = movePosInfo.MoveDir;
            _map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));


            S2C_Move res = new S2C_Move
            {
                PlayerId = player._playerInfo.PlayerId,
                PosInfo = packet.PosInfo,
            };

            foreach (var p in _players.Values)
            {
                p.Send(res);
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
            PlayerInfo info = player._playerInfo;
            if (info.PosInfo.State != CreatureState.Idle)
            {
                Log.Error("HandleSkill player is not idle");
                return;
            }

            info.PosInfo.State = CreatureState.Skill;

            S2C_Skill res = new S2C_Skill
            {
                PlayerId = player._playerInfo.PlayerId,
                SkillInfo = new SkillInfo
                {
                    SkillId = 1,
                }
            };

            foreach (var p in _players.Values)
                p.Send(res);

            //BroadCast(res);

            // TODO : 데미지 판정
            var skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
            Player target = _map.Find(skillPos);
            if (target != null)
            {
               Log.Info("Player Hit");
            }
        }
    }

    public void BroadCast(IMessage packet)
    {
        lock (_lock)
        {
            foreach (var player in _players.Values)
            {
                player.Send(packet);
            }
        }
    }
    //List<ClientSession> _sessions = new List<ClientSession>();
    //JobQueue _jobQueue = new JobQueue();
    //List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    //public void Push(Action job)
    //{
    //    _jobQueue.Push(job);
    //}

    //public void Flush()
    //{
    //    foreach (ClientSession session in _sessions)
    //    {
    //        session.Send(_pendingList);
    //    }
    //}

    //public void Broadcast(ArraySegment<byte> segment)
    //{
    //    _pendingList.Add(segment);
    //}

    //public void Enter(ClientSession session)
    //{
    //    _sessions.Add(session);

    //    session.Zone = this;

    //    {
    //        // 모든 플레이어 목록 전송
    //        S2C_PlayerList res = new S2C_PlayerList();
    //        foreach (ClientSession s in _sessions)
    //        {
    //            res.Players.Add(new TPlayer
    //            {
    //                IsSelf = (s == session),
    //                PlayerId = s.SessionId,
    //                PosX = s.PosX,
    //                PosY = s.PosY,
    //                PosZ = s.PosZ
    //            });
    //        }
    //        res.Result = (int)ErrorType.Success;

    //        ushort size = (ushort)res.CalculateSize();

    //        byte[] sendBuffer = new byte[size + 4];
    //        Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));

    //        ushort protocolId = PacketManager.Instance.GetMessageId(res.GetType());
    //        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));

    //        Array.Copy(res.ToByteArray(), 0, sendBuffer, 4, size);
    //        var buff = new ArraySegment<byte>(sendBuffer);
    //        session.Send(buff);
    //    }


    //    // 새로운 플레이어 입장을 모두에게 알림
    //    {
    //        S2C_BroadcastEnterGame enter = new S2C_BroadcastEnterGame();

    //        enter.PlayerId = session.SessionId;
    //        enter.PosX = session.PosX;
    //        enter.PosY = session.PosY;
    //        enter.PosZ = session.PosZ;

    //        enter.Result = (int)ErrorType.Success;

    //        ushort size = (ushort)enter.CalculateSize();

    //        byte[] sendBuffer = new byte[size + 4];
    //        Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));

    //        ushort protocolId = PacketManager.Instance.GetMessageId(enter.GetType());
    //        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));

    //        Array.Copy(enter.ToByteArray(), 0, sendBuffer, 4, size);
    //        var buff = new ArraySegment<byte>(sendBuffer);

    //        Broadcast(buff);
    //    }
    //}

    //public void Leave(ClientSession session)
    //{
    //    // 플레이어 제거
    //    _sessions.Remove(session);

    //    // 모두에게 알림
    //    S2C_BroadcastLeaveGame leave = new S2C_BroadcastLeaveGame();
    //    leave.PlayerId = session.SessionId;

    //    leave.Result = (int)ErrorType.Success;

    //    ushort size = (ushort)leave.CalculateSize();
    //    byte[] sendBuffer = new byte[size + 4];
    //    Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
    //    ushort protocolId = (ushort)MsgId.S2CBroadcastleavegame;
    //    Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
    //    Array.Copy(leave.ToByteArray(), 0, sendBuffer, 4, size);
    //    var buff = new ArraySegment<byte>(sendBuffer);


    //    Broadcast(buff);
    //}

    //public void Move(ClientSession session, C2S_Move packet)
    //{
    //    session.PosX = packet.PosX;
    //    session.PosY = packet.PosY;
    //    session.PosZ = packet.PosZ;

    //    S2C_BroadcastMove move = new S2C_BroadcastMove();
    //    move.PlayerId = session.SessionId;
    //    move.PosX = session.PosX;
    //    move.PosY = session.PosY;
    //    move.PosZ = session.PosZ;

    //    // 모두에게 알림
    //    move.Result = (int)ErrorType.Success;

    //    ushort size = (ushort)move.CalculateSize();
    //    byte[] sendBuffer = new byte[size + 4];
    //    Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
    //    ushort protocolId = (ushort)MsgId.S2CBroadcastmove;
    //    Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
    //    Array.Copy(move.ToByteArray(), 0, sendBuffer, 4, size);
    //    var buff = new ArraySegment<byte>(sendBuffer);

    //    Broadcast(buff);
    //}
}
