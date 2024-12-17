using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DB;

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