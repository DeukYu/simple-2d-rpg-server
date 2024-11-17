using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Diagnostics.CodeAnalysis;

namespace CS_Server;

public class Monster : GameObject
{
    public Monster()
    {
        ObjectType = GameObjectType.Monster;

        // 
        StatInfo.Level = 1;
        StatInfo.Hp = 100;
        StatInfo.MaxMp = 100;
        StatInfo.Speed = 5.0f;

        State = CreatureState.Idle;
    }

    // FSM (Finite State Machine)
    public override void Update()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Move:
                UpdateMove();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    Player _target;
    int _searchCellDist = 10;
    int _chaseCellDist = 20;
    long _nextSearchTick = 0;
    protected virtual void UpdateIdle()
    {
        if (_nextSearchTick > Environment.TickCount64)
            return;

        _nextSearchTick = Environment.TickCount64 + 1000;

        var target = _zone.FindPlayer(p =>
        {
            var dir = p.CellPos - CellPos;
            return dir.cellDistance <= _searchCellDist;
        });

        if (target == null)
        {
            return;
        }

        _target = target;
        State = CreatureState.Move;
    }

    int _skillRange = 1;
    long _nextMoveToTick = 0;
    protected virtual void UpdateMove()
    {
        if (_nextMoveToTick > Environment.TickCount64)
        {
            return;
        }

        int moveTick = (int)(1000 / Speed);
        _nextMoveToTick = Environment.TickCount64 + 1000;

        if (_target == null || _target._zone != _zone)
        {
            _target = null;
            State = CreatureState.Idle;
            BroadCastMove();
            return;
        }

        var dir = _target.CellPos - CellPos;
        int dist = dir.cellDistance;
        if (dist == 0 || dist > _chaseCellDist)
        {
            _target = null;
            State = CreatureState.Idle;
            BroadCastMove();
            return;
        }

        var paths = _zone.Map.FindPath(CellPos, _target.CellPos, false);
        if (paths.Count < 2 || paths.Count > _chaseCellDist)
        {
            _target = null;
            State = CreatureState.Idle;
            BroadCastMove();
            return;
        }

        // 스킬로 넘어갈지 체크
        if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))
        {
            _coolTick = 0;
            State = CreatureState.Skill;
            return;
        }

        // 이동
        Dir = GetDirFromVec(paths[1] - CellPos);
        _zone.Map.ApplyMove(this, paths[1]);

        // 다른 플레이어한테 알림
        BroadCastMove();
    }

    void BroadCastMove()
    {
        S2C_Move movePacket = new S2C_Move() { ObjectId = Id, PosInfo = PosInfo };
        _zone.BroadCast(movePacket);
    }

    long _coolTick = 0;
    protected virtual void UpdateSkill()
    {
        if (_coolTick == 0)
        {
            if (_target == null || _target._zone != _zone || _target.Hp == 0)
            {
                _target = null;
                State = CreatureState.Move;
                BroadCastMove();
                return;
            }

            var dir = (_target.CellPos - CellPos);
            int dist = dir.cellDistance;
            bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));
            if (canUseSkill == false)
            {
                State = CreatureState.Move;
                BroadCastMove();
                return;
            }

            var lookDir = GetDirFromVec(dir);
            if (Dir != lookDir)
            {
                Dir = lookDir;
                BroadCastMove();
            }

            if (DataManager.SkillDict.TryGetValue(1, out var skillData) == false)
            {
                Log.Error("Skill data is not exist");
                State = CreatureState.Move;
                BroadCastMove();
                return;
            }

            _target.OnDamaged(this, skillData.Damage + StatInfo.Attack);

            S2C_Skill skillPacket = new S2C_Skill
            {
                ObjectId = Id,
                SkillInfo = new SkillInfo
                {
                    SkillId = 1,
                }
            };
            _zone.BroadCast(skillPacket);

            int coolTick = (int)(skillData.Cooltime * 1000);
            _coolTick = Environment.TickCount64 + coolTick;
        }

        if (_coolTick > Environment.TickCount64)
        {
            return;
        }

        _coolTick = 0;
    }

    protected virtual void UpdateDead()
    {
        // 
    }
}
