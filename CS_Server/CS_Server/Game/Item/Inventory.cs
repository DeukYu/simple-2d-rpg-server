﻿namespace CS_Server;

public class Inventory
{
    Dictionary<int, Item> _items = new Dictionary<int, Item>();

    public void Add(Item item)
    {
        _items.Add(item.TemplateId, item);
    }

    public Item Get(int itemId)
    {
        Item item = null;
        _items.TryGetValue(itemId, out item);
        return item;
    }

    public Item Find(Func<Item, bool> condition)
    {
        foreach(Item item in _items.Values)
        {
            if (condition.Invoke(item))
            {
                return item;
            }
        }
        return null;
    }
}
