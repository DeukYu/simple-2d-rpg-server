using Google.Protobuf.Enum;

namespace CS_Server;

public class Inventory
{
    public Dictionary<long, Item> Items { get; } = new Dictionary<long, Item>(); // Dictionary<ItemUid, Item>

    public void Add(Item item)
    {
        Items.Add(item.ItemUid, item);
    }

    public bool TryGet(long itemUid, out Item item)
    {
        return Items.TryGetValue(itemUid, out item);
    }

    public Item? Find(Func<Item, bool> condition)
    {
        foreach (Item item in Items.Values)
        {
            if (condition.Invoke(item))
            {
                return item;
            }
        }
        return null;
    }

    public int? GetEmptySlot()
    {
        for (int slot = 0; slot < 20; slot++)
        {
            var item = Items.Values.FirstOrDefault(item => item.Slot == slot);
            if (item == null)
                return slot;
        }
        return null;
    }

    public bool TryGetEquipItem(ItemType itemType, out Item item)
    {
        item = null;

        if (itemType == ItemType.Weapon)
        {
            item = Find(i => i.Equipped && i.ItemType == ItemType.Weapon);
            return item != null;
        }
        else if (itemType == ItemType.Armor)
        {
            var armorType = ((Armor)item).ArmorType;
            item = Find(i => i.Equipped && i.ItemType == ItemType.Armor && ((Armor)i).ArmorType == armorType);
            return item != null;
        }

        return false;
    }
}
