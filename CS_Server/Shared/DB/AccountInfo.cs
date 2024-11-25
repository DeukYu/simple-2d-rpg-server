﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared;

[Table("account_info")]
public class AccountInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public string AccountName { get; set; } = string.Empty;
    public ICollection<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();
}

[Table("player_info")]
public class PlayerInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public string PlayerName { get; set; } = string.Empty;
    
    [ForeignKey(nameof(AccountInfo))]
    public long AccountId { get; set; } = 0;
    public AccountInfo AccountInfo { get; set; } = new AccountInfo();
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
    public PlayerInfo PlayerInfo { get; set; } = new PlayerInfo();
}