using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ServerCore;

namespace Shared.DB;



[Table("player_info")]
public class PlayerInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public string PlayerName { get; set; } = string.Empty;
    public int Level { get; set; } = 0;
    public int Hp { get; set; } = 0;
    public int MaxHp { get; set; } = 0;
    public int Mp { get; set; } = 0;
    public int MaxMp { get; set; } = 0;
    public int Attack { get; set; } = 0;
    public float Speed { get; set; } = 0;
    public int TotalExp { get; set; } = 0;
    public ICollection<PlayerItemInfo> Items { get; set; } = new List<PlayerItemInfo>();

    [ForeignKey(nameof(AccountInfo))]
    public long AccountId { get; set; } = 0;
}

public static class PlayerInfoExtensions
{
    public static bool IsPlayerNameExist(this DbSet<PlayerInfo> playerInfo, string playerName)
    {
        return playerInfo.AsNoTracking().Any(x => x.PlayerName == playerName);
    }

    public static PlayerInfo CreatePlayer(this DbSet<PlayerInfo> playerInfo, string playerName, long accountId, StatData statData)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Log.Error("Invalid PlayerName");
            return null;
        }

        var player = new PlayerInfo
        {
            PlayerName = playerName,
            Level = statData.Level,
            Hp = statData.MaxHp,
            MaxHp = statData.MaxHp,
            Mp = statData.MaxMp,
            MaxMp = statData.MaxMp,
            Attack = statData.Attack,
            Speed = statData.Speed,
            TotalExp = 0,
            AccountId = accountId,
        };
        playerInfo.Add(player);
        return player;
    }
}