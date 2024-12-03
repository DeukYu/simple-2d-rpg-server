using Google.Protobuf.Common;
using Shared;

namespace CS_Server;

public static class LobbyPlayerInfoFactory
{
    public static LobbyPlayerInfo CreateLobbyPlayerInfo(PlayerInfo playerInfo, PlayerStatInfo playerStatInfo)
    {
        return new LobbyPlayerInfo
        {
            PlayerId = playerInfo.Id,
            Name = playerInfo.PlayerName,
            StatInfo = StatInfoMapper.MapToStatInfo(playerStatInfo)
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
            TotalExp = playerStatInfo.TotalExp
        };
    }
}