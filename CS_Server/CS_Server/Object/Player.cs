using Google.Protobuf.Enum;

namespace CS_Server;

public class Player : GameObject
{
    public ClientSession? Session { get; set; }

    public Player()
    {
        ObjectType = GameObjectType.Player;
    }
}
