namespace CS_Server;

public class Inventory
{
    public Dictionary<int, Item> Items { get; } = new Dictionary<int, Item>(); // Dictionary<TemplateId, Item>

    public void Add(Item item)
    {
        Items.Add(item.TemplateId, item);
    }

    public Item? Get(int itemId)
    {
        return Items.TryGetValue(itemId, out var item) ? item : null;
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
}
