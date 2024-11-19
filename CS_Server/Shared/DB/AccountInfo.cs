using System.ComponentModel.DataAnnotations;
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
    public long AccountId { get; set; } = 0;
    public string PlayerName { get; set; } = string.Empty;
}