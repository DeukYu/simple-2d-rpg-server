using Google.Protobuf.Enum;
using ServerCore;

namespace CS_Server;

public class Inventory
{
    public Dictionary<long, Item> Items { get; } = new Dictionary<long, Item>(); // Dictionary<ItemUid, Item>

    public void Add(Item item)
    {
        if (Items.ContainsKey(item.ItemUid))
        {
            Log.Error($"Item with UID {item.ItemUid} already exists in the inventory.");
            return;
        }

        Items.Add(item.ItemUid, item);
    }

    public bool TryGet(long itemUid, out Item item)
    {
        return Items.TryGetValue(itemUid, out item);
    }

    public Item? Find(Func<Item, bool> condition)
    {
        return Items.Values.FirstOrDefault(condition);
    }

    public int? GetEmptySlot(int maxSlots = 20)
    {
        for (int slot = 0; slot < maxSlots; slot++)
        {
            if (!Items.Values.Any(item => item.Slot == slot))
                return slot;
        }
        return null;
    }

    public bool TryGetEquipItem(ItemType itemType, out Item? item)
    {
        item = null;

        switch (itemType)
        {
            case ItemType.Weapon:
                item = Find(i => i.Equipped && i.ItemType == ItemType.Weapon);
                break;
            case ItemType.Armor:
                var armorType = ((Armor)item).ArmorType;
                item = Find(i => i.Equipped && i.ItemType == ItemType.Armor && ((Armor)i).ArmorType == armorType);
                break;
            default:
                item = Find(i => i.Equipped && i.ItemType == itemType);
                break;
        }
        return false;
    }
}
