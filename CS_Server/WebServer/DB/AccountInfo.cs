using Google.Protobuf.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServer;

[Table("account_info")]
public class AccountInfo
{
    [Key]
    public long Id { get; set; } = 0;
    public string AccountName { get; set; } = string.Empty;
}