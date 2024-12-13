using Google.Protobuf.Common;
using Shared;

namespace CS_Server;

public static class LobbyPlayerInfoFactory
{
    public static LobbyPlayerInfo CreateLobbyPlayerInfo(PlayerInfo playerInfo, PlayerStatInfo playerStatInfo)
    {
        return new LobbyPlayerInfo
        {
            PlayerUid = playerInfo.Id,
            Name = playerInfo.PlayerName,
            StatInfo = StatInfoMapper.MapToStatInfo(playerStatInfo)
        };
    }

    public static LobbyPlayerInfo CreateLobbyPlayerInfo(PlayerInfo playerInfo, StatData statData)
    {
        return new LobbyPlayerInfo
        {
            PlayerUid = playerInfo.Id,
            Name = playerInfo.PlayerName,
            StatInfo = StatInfoMapper.MapToStatData(statData)
        };
    }
}

public static class StatInfoMapper
{
    public static StatInfo MapToStatInfo(PlayerStatInfo playerStatInfo)
    {
        return new StatInfo
        {
            Level = playerStatInfo.Level,
            Hp = playerStatInfo.Hp,
            MaxHp = playerStatInfo.MaxHp,
            Mp = playerStatInfo.Mp,
            MaxMp = playerStatInfo.MaxMp,
            Attack = playerStatInfo.Attack,
            Speed = playerStatInfo.Speed,
            TotalExp = playerStatInfo.TotalExp  // TODO : 추후에 수정해야 함
        };
    }
    public static StatInfo MapToStatData(StatData statData)
    {
        return new StatInfo
        {
            Level = statData.Level,
            Hp = statData.MaxHp,
            MaxHp = statData.MaxHp,
            Mp = statData.MaxMp,
            MaxMp = statData.MaxMp,
            Attack = statData.Attack,
            Speed = statData.Speed,
            TotalExp = 0    // TODO : 추후에 수정해야 함
        };
    }
}

public static class PlayerStatInfoFactory
{
    public static PlayerStatInfo CreatePlayerStatInfo(long playerId, StatData statData)
    {
        return new PlayerStatInfo
        {
            PlayerId = playerId,
            Level = statData.Level,
            Hp = statData.MaxHp,
            MaxHp = statData.MaxHp,
            Mp = statData.MaxMp,
            MaxMp = statData.MaxMp,
            Attack = statData.Attack,
            Speed = statData.Speed,
            TotalExp = 0    // TODO : 추후에 수정해야 함
        };
    }
}