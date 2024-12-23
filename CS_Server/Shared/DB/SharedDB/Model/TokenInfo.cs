using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DB;

[Table("token_info")]
public class TokenInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public long AccountId { get; set; } = 0;
    public string Token { get; set; } = string.Empty;
    public DateTime Expired { get; set; } = DateTime.MinValue;
}
