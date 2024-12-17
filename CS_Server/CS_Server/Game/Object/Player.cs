using CS_Server.Game;
using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;
using Shared.DB;

namespace CS_Server;

public class Player : GameObject
{
    public long PlayerUid { get; set; }
    public ClientSession? Session { get; set; }
    public VisionCube? Vision { get; private set; }
    public Inventory Inven { get; private set; } = new Inventory();
    public int WeaponDamage { get; private set; } = 0;
    public int ArmorDefense { get; private set; } = 0;
    public override int TotalAttack { get { return StatInfo.Attack + WeaponDamage; } }
    public override int TotalDefense { get { return 0 + ArmorDefense; } }
    public Player()
    {
        ObjectType = GameObjectType.Player;
        Vision = new VisionCube(this);
    }

    private List<ItemInfo> GetPlayerItemsFromDb(long playerUid)
    {
        using (var db = new AccountDB())
        {
            var items = db.ItemInfo
                .Where(i => i.PlayerId == playerUid)
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
            return itemInfoList;
        }
    }

    public void SetPlayer(ClientSession session, LobbyPlayerInfo lobbyPlayerInfo)
    {
        PlayerUid = lobbyPlayerInfo.PlayerUid;

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
        var itemInfoList = GetPlayerItemsFromDb(PlayerUid);
        SendItemListPacket(itemInfoList);
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

        // 시야각 업데이트 처리
        Vision.Update();
    }

    public void OnLeaveGame()
    {
        DbTransaction.UpdatePlayerStatus(this, Zone);
    }

    public void HandleEquipItem(long itemUid, bool equipped)
    {
        // 인벤토리에서 아이템 확인
        if (Inven.TryGet(itemUid, out var item) == false)
        {
            Log.Error("HandleEquipItem : item is null");
            return;
        }

        // 아이템 타입 확인
        if (item.ItemType == ItemType.Consumable)
        {
            Log.Error("HandleEquipItem : item is consumable");
            return;
        }

        // 장착 여부 확인
        if (equipped)
        {
            // 이미 장착된 아이템이 있는지 확인하고 해제한다.
            if (Inven.TryGetEquipItem(item.ItemType, out var unequippedItem))
            {
                // 메모리 선적용
                unequippedItem.Equipped = false;
                // DB 적용
                DbTransaction.EquipItemNotify(this, unequippedItem);
                // 클라 전송
                SendEquipItemPacket(unequippedItem.ItemUid, false);
            }
        }

        // 장착 처리
        {
            // 메모리 선적용
            item.Equipped = equipped;
            // DB 적용
            DbTransaction.EquipItemNotify(this, item);
            // 클라 전송
            SendEquipItemPacket(itemUid, equipped);
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

    public void SendEquipItemPacket(long itemUid, bool equipped)
    {
        var equipItemRes = new S2C_EquipItem
        {
            ItemUid = itemUid,
            Equipped = equipped,
        };
        Session.Send(equipItemRes);
    }
}