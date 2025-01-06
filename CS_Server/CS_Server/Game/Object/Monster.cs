using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public class Monster : GameObject
{
    public int TemplateId { get; private set; }
    private Dictionary<CreatureState, Action> StateHandlers;

    private Player? _target;    // 타겟 
    private int _searchCellDist = 10;   // 검색 범위
    private int _chaseCellDist = 20;    // 추적 범위
    private long _nextSearchTick = 0;   // 다음 타겟 검색 시간
    private int _skillRange = 1;        // 스킬 범위
    private long _nextMoveToTick = 0;   // 다음 이동 시간
    private long _coolTick = 0;         // 스킬 쿨타임

    private JobBase? _job;
    public Monster()
    {
        ObjectType = GameObjectType.Monster;
        StateHandlers = new Dictionary<CreatureState, Action>
        {
            { CreatureState.Idle, UpdateIdle },
            { CreatureState.Move, UpdateMove },
            { CreatureState.Skill, UpdateSkill },
            { CreatureState.Dead, UpdateDead },
        };

        _target = null;
        _job = null;
    }
    public void Init(int templateId)
    {
        TemplateId = templateId;

        if (DataManager.MonterDataDict.TryGetValue(templateId, out var monsterData) == false)
        {
            Log.Error($"Failed to find monster data. TemplateId{templateId}");
            return;
        }

        StatInfo.MergeFrom(monsterData.Stat);
        State = CreatureState.Idle;
    }

    // 유효한 타겟인지 체크
    private bool IsTargetValid()
    {
        return _target != null && _target.Zone == Zone && _target.Hp > 0;
    }

    // 상태 변환
    private void TransitionToIdle()
    {
        _target = null;
        State = CreatureState.Idle;
        BroadCastMove();
    }

    private void TransitionToSkill()
    {
        _coolTick = 0;
        State = CreatureState.Skill;
    }

    // FSM (Finite State Machine)
    public override void Update()
    {
        StateHandlers[State]?.Invoke();

        if (Zone != null)
            _job = Zone.ScheduleJobAfterDelay(200, Update);
    }

    protected virtual void UpdateIdle()
    {
        const int SearchIntervalMs = 1000;

        // 다음 타겟 검색 시간이 아니면 리턴
        if (_nextSearchTick > Environment.TickCount64)
            return;

        // 다음 검색 시간을 설정
        _nextSearchTick = Environment.TickCount64 + SearchIntervalMs;

        // 타겟 검색
        var target = Zone.FindClosedPlayer(CellPos, _searchCellDist);
        if (target == null)
            return;

        // 타겟 설정 및 상태 전환
        _target = target;
        State = CreatureState.Move;
    }

    protected virtual void UpdateMove()
    {
        if (_nextMoveToTick > Environment.TickCount64)
        {
            return;
        }

        int moveTick = (int)(1000 / Speed);
        _nextMoveToTick = Environment.TickCount64 + 1000;

        if (!IsTargetValid())
        {
            TransitionToIdle();
            return;
        }

        var dir = _target.CellPos - CellPos;
        int dist = dir.cellDistance;
        if (dist == 0 || dist > _chaseCellDist)
        {
            TransitionToIdle();
            return;
        }

        var paths = Zone.Map.FindPath(CellPos, _target.CellPos, checkObjects: true);
        if (paths.Count < 2 || paths.Count > _chaseCellDist)
        {
            TransitionToIdle();
            return;
        }

        // 스킬로 넘어갈지 체크
        if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))
        {
            TransitionToSkill();
            return;
        }

        // 이동
        Dir = GetDirFromVec(paths[1] - CellPos);
        Zone.Map.ApplyMove(this, paths[1]);

        // 다른 플레이어한테 알림
        BroadCastMove();
    }

    void BroadCastMove()
    {
        Zone.BroadCast(CellPos, new S2C_Move() { ObjectId = Id, PosInfo = PosInfo });
    }

    protected virtual void UpdateSkill()
    {
        if (_coolTick == 0)
        {
            if (_target == null || _target.Zone != Zone || _target.Hp == 0)
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

            _target.OnDamaged(this, skillData.Damage + TotalAttack);

            S2C_Skill skillPacket = new S2C_Skill
            {
                ObjectId = Id,
                SkillInfo = new SkillInfo
                {
                    SkillId = 1,
                }
            };
            Zone.BroadCast(CellPos, skillPacket);

            int coolTick = (int)(skillData.Cooltime * 1000);
            _coolTick = Environment.TickCount64 + coolTick;
        }

        if (_coolTick > Environment.TickCount64)
        {
            return;
        }

        _coolTick = 0;
    }

    protected virtual void UpdateDead() { }

    private void CancelJob()
    {
        if (_job != null)
        {
            _job.IsCanceled = true;
            _job = null;
        }
    }

    public override void OnDead(GameObject attacker)
    {
        CancelJob();

        base.OnDead(attacker);

        var owner = attacker.GetOwner();

        if (attacker.GetOwner() is Player player)
        {
            var rewardData = GetRewardData(TemplateId);
            if (rewardData != null)
                DbTransaction.RewardPlayer(player, rewardData, Zone);
        }
    }

    private RewardData GetRewardData(int templateId)
    {
        if (DataManager.MonterDataDict.TryGetValue(templateId, out var monsterData) == false)
        {
            Log.Error("Failed to find monster data");
            return null;
        }

        if (DataManager.RewardDataDict.TryGetValue(monsterData.Id, out var rewardDatas) == false)
        {
            Log.Error("Failed to find reward data");
            return null;
        }

        int sum = 0;
        int rand = new Random().Next(0, 101);

        foreach (var rewardData in rewardDatas)
        {
            sum += rewardData.Probability;
            if (rand <= sum)
            {
                return rewardData;
            }
        }
        return null;
    }
}
