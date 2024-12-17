using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DB;



[Table("player_info")]
public class PlayerInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public string PlayerName { get; set; } = string.Empty;
    public ICollection<PlayerItemInfo> Items { get; set; } = new List<PlayerItemInfo>();
    public PlayerStatInfo StatInfo { get; set; } = new PlayerStatInfo();

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
    public static PlayerInfo CreatePlayer(this DbSet<PlayerInfo> playerInfo, string playerName, long accountId)
    {
        var player = new PlayerInfo
        {
            PlayerName = playerName,
            AccountId = accountId
        };

        playerInfo.Add(player);
        return player;
    }
}

[Table("player_stat_info")]
public class PlayerStatInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public int Level { get; set; } = 0;
    public int Hp { get; set; } = 0;
    public int MaxHp { get; set; } = 0;
    public int Mp { get; set; } = 0;
    public int MaxMp { get; set; } = 0;
    public int Attack { get; set; } = 0;
    public float Speed { get; set; } = 0;
    public int TotalExp { get; set; } = 0;

    [ForeignKey(nameof(PlayerInfo))]
    public long PlayerId { get; set; } = 0;
    public virtual PlayerInfo PlayerInfo { get; set; } = new PlayerInfo();
}

public static class PlayerStatInfoExtensions
{
    public static PlayerStatInfo CreatePlayerStat(this DbSet<PlayerStatInfo> playerStatInfo, long playerUid, StatData statData)
    {
        var playerStat = new PlayerStatInfo
        {
            PlayerId = playerUid,
            Level = statData.Level,
            Hp = statData.MaxHp,
            MaxHp = statData.MaxHp,
            Mp = statData.MaxMp,
            MaxMp = statData.MaxMp,
            Attack = statData.Attack,
            Speed = statData.Speed,
            TotalExp = 0
        };
        playerStatInfo.Add(playerStat);
        return playerStat;
    }
}

[Table("player_item_info")]
public class PlayerItemInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public int ItemId { get; set; } = 0;
    public int Count { get; set; } = 0;
    public int Slot { get; set; } = 0;
    public bool Equipped { get; set; } = false;

    [ForeignKey(nameof(PlayerInfo))]
    public long PlayerId { get; set; } = 0;
    public virtual PlayerInfo PlayerInfo { get; set; } = new PlayerInfo();
}