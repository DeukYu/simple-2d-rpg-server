using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using Shared;

namespace CS_Server;

public class Player : GameObject
{
    public long PlayerId { get; set; }  // DB ID
    public ClientSession? Session { get; set; }
    public Inventory Inven { get; private set; } = new Inventory();

    public Player()
    {
        ObjectType = GameObjectType.Player;
    }

    public void SetPlayer(ClientSession session, LobbyPlayerInfo lobbyPlayerInfo)
    {
        PlayerId = lobbyPlayerInfo.PlayerId;

        // Info
        Info.PosInfo.State = CreatureState.Idle;
        Info.PosInfo.MoveDir = MoveDir.Down;
        Info.PosInfo.PosX = 0;
        Info.PosInfo.PosY = 0;

        // Stat
        StatInfo.Level = lobbyPlayerInfo.StatInfo.Level;
        StatInfo.Hp = lobbyPlayerInfo.StatInfo.Hp;
        StatInfo.MaxHp = lobbyPlayerInfo.StatInfo.MaxHp;
        StatInfo.Mp = lobbyPlayerInfo.StatInfo.Mp;
        StatInfo.MaxMp = lobbyPlayerInfo.StatInfo.MaxMp;
        StatInfo.Attack = lobbyPlayerInfo.StatInfo.Attack;
        StatInfo.Exp = 0;
        StatInfo.TotalExp = lobbyPlayerInfo.StatInfo.TotalExp;

        // Session
        Session = session;

        // Item
        using (var db = new AccountDB())
        {
            var items = db.ItemInfo
                .Where(i => i.PlayerId == PlayerId)
                .ToList();

            var itemInfoList = new List<ItemInfo>();

            foreach (var item in items)
            {
                if (Item.MakeItem(item, out var newItem))
                {
                    Inven.Add(newItem);
                    itemInfoList.Add(newItem.Info);
                }
            }
            SendItemListPacket(itemInfoList);
        }
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

    public void OnLeaveGame()
    {
        DbTransaction.UpdatePlayerStatus(this, _zone);
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

    public void SendItemListPacket(List<ItemInfo> itemList)
    {
        var itemListRes = new S2C_ItemList
        {
            Items = { itemList },
        };
        Session.Send(itemListRes);
    }

    public void SendAddItemPacket(ItemInfo itemInfo)
    {
        var addItemRes = new S2C_AddItem
        {
            Items = { itemInfo },
        };
        Session.Send(addItemRes);
    }
}