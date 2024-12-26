using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Config;

public class ServerConfig
{
    public string ServerName { get; set; } = string.Empty;
    public int ServerPort { get; set; } = 0;
    public int WebServerPort { get; set; } = 0;
}
