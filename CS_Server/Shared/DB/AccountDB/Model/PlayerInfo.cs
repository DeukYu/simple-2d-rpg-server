using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
    public ICollection<PlayerItemInfo> Items { get; set; }

    [ForeignKey(nameof(AccountInfo))]
    public long AccountId { get; set; } = 0;
    public virtual AccountInfo AccountInfo { get; set; } = new AccountInfo();
}

public static class PlayerInfoExtensions
{
    public static bool IsPlayerNameExist(this DbSet<PlayerInfo> playerInfo, string playerName)
    {
        return playerInfo.Any(x => x.PlayerName == playerName);
    }
    public static PlayerInfo CreatePlayer(this DbSet<PlayerInfo> playerInfo, string playerName, long accountId, StatData statData)
    {
        var player = new PlayerInfo
        {
            PlayerName = playerName,
            AccountId = accountId,
            Level = statData.Level,
            Hp = statData.MaxHp,
            MaxHp = statData.MaxHp,
            Mp = statData.MaxMp,
            MaxMp = statData.MaxMp,
            Attack = statData.Attack,
            Speed = statData.Speed,
            TotalExp = 0,
        };
        playerInfo.Add(player);
        return player;
    }
}