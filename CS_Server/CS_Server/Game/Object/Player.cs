using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;

namespace CS_Server;

public class Player : GameObject
{
    public long PlayerId { get; set; }  // DB ID
    public ClientSession? Session { get; set; }
    public Inventory Inven { get; private set; } = new Inventory();

    public Player()
    {
        ObjectType = GameObjectType.Player;
        Speed = 10.0f;
    }

    public override void OnDamaged(GameObject attacker, int damage)
    {
        base.OnDamaged(attacker, damage);
    }

    public override void OnDead(GameObject attacker)
    {
        base.OnDead(attacker);
    }

    public void OnEnterGame(List<ObjectInfo> objects)
    {
        // 게임 입장 처리
        SendEnterGamePacket();
        SendSpawnPacket(objects);
    }

    private void SendEnterGamePacket()
    {
        var enterGameRes = new S2C_EnterGame
        {
            Result = (int)ErrorType.Success,
            ObjectInfo = Info,
        };
        Session.Send(enterGameRes);
    }

    private void SendSpawnPacket(List<ObjectInfo> objects)
    {
        if (objects.Count > 0)
        {
            var spawnRes = new S2C_Spawn
            {
                Objects = { objects },
            };
            Session.Send(spawnRes);
        }
    }

    public void OnLeaveGame()
    {
       DbTransaction.UpdatePlayerStatus(this, _zone);
    }
}
