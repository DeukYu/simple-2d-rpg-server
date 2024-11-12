using Google.Protobuf;
using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using System.Net.Http.Headers;

namespace CS_Server;

public class Player : GameObject
{
    public ClientSession Session { get; set; }

    public Player()
    {
        ObjectType = GameObjectType.Player;
    }
}
