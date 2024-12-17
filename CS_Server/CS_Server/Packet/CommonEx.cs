using Google.Protobuf.Common;
using Shared;
using Shared.DB;

namespace CS_Server;

public static class LobbyPlayerInfoFactory
{
    public static LobbyPlayerInfo CreateLobbyPlayerInfo(PlayerInfo playerInfo)
    {
        return new LobbyPlayerInfo
        {
            PlayerUid = playerInfo.Id,
            Name = playerInfo.PlayerName,
            StatInfo = new StatInfo
            {
                Level = playerInfo.Level,
                Hp = playerInfo.Hp,
                MaxHp = playerInfo.MaxHp,
                Mp = playerInfo.Mp,
                MaxMp = playerInfo.MaxMp,
                Attack = playerInfo.Attack,
                Speed = playerInfo.Speed,
                TotalExp = playerInfo.TotalExp,
            }
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