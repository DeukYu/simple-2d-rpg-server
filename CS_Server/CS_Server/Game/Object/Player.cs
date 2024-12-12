using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using Shared;

namespace CS_Server;

public class Player : GameObject
{
    public long PlayerId { get; set; }  // DB ID
    public ClientSession? Session { get; set; }
    public Inventory Inven { get; private set; } = new Inventory();
    public int WeaponDamage { get; private set; } = 0;
    public int ArmorDefense { get; private set; } = 0;
    public override int TotalAttack { get { return StatInfo.Attack + WeaponDamage; } }
    public override int TotalDefense { get { return 0 + ArmorDefense; } }   // TODO
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

    public void HandleEquipItem(int itemId, bool equipped)
    {
        var item = Inven.Get(itemId);
        if (item == null)
        {
            Log.Error("HandleEquipItem : item is null");
            return;
        }

        if (item.ItemType == ItemType.Consumable)
        {
            Log.Error("HandleEquipItem : item is consumable");
            return;
        }

        if (equipped)
        {
            Item unequippedItem = null;
            if (item.ItemType == ItemType.Weapon)
            {
                unequippedItem = Inven
                    .Find(i => i.Equipped && i.ItemType == ItemType.Weapon);
            }
            else if (item.ItemType == ItemType.Armor)
            {
                ArmorType armorType = ((Armor)item).ArmorType;
                unequippedItem = Inven
                    .Find(i => i.Equipped && i.ItemType == ItemType.Armor
                    && ((Armor)i).ArmorType == armorType);
            }

            if (unequippedItem != null)
            {
                // 메모리 선적용
                unequippedItem.Equipped = false;

                // DB 적용
                DbTransaction.EquipItemNotify(this, unequippedItem);

                // 클라 전송
                S2C_EquipItem equipItemPacket = new S2C_EquipItem();
                equipItemPacket.ItemId = (Int32)unequippedItem.ItemId;  // TODO : int -> long
                equipItemPacket.Equipped = unequippedItem.Equipped;
                Session.Send(equipItemPacket);
            }
        }

        {
            // 메모리 선적용
            item.Equipped = equipped;

            // DB 적용
            DbTransaction.EquipItemNotify(this, item);

            // 클라 전송
            S2C_EquipItem equipItemPacket = new S2C_EquipItem();
            equipItemPacket.ItemId = itemId;
            equipItemPacket.Equipped = equipped;
            Session.Send(equipItemPacket);
        }
        RefreshAdditionalStat();
    }

    public void RefreshAdditionalStat()
    {
        WeaponDamage = 0;
        ArmorDefense = 0;

        foreach (var item in Inven.Items.Values)
        {
            if (item.Equipped == false)
                continue;

            switch (item.ItemType)
            {
                case ItemType.Weapon:
                    WeaponDamage += ((Weapon)item).Damage;
                    break;
                case ItemType.Armor:
                    ArmorDefense += ((Armor)item).Defense;
                    break;
            }
        }
    }
}