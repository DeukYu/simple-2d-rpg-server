using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DB;

[Table("server_config_info")]
public class ServerConfigInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public int Congestion { get; set; } = 0;
}
