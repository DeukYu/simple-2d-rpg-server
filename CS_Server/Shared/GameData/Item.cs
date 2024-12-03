using Google.Protobuf.Enum;

namespace Shared;

[Serializable]
public class ItemData : ICsvConvertible
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ItemType ItemType { get; set; }

    public virtual void FromCsv(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
        ItemType = (ItemType)Enum.Parse(typeof(ItemType), values[2], true);
    }
}
[Serializable]
public class WeaponData : ItemData
{
    public WeaponType WeaponType { get; set; }
    public int Damage { get; set; }
    public override void FromCsv(string[] values)
    {
        base.FromCsv(values);
        WeaponType = (WeaponType)Enum.Parse(typeof(WeaponType), values[3], true);
        Damage = int.Parse(values[4]);
    }
}
[Serializable]
public class ArmorData : ItemData
{
    public ArmorType ArmorType { get; set; }
    public int Defense { get; set; }
    public override void FromCsv(string[] values)
    {
        base.FromCsv(values);
        ArmorType = (ArmorType)Enum.Parse(typeof(ArmorType), values[3], true);
        Defense = int.Parse(values[4]);
    }
}
[Serializable]
public class ConsumableData : ItemData
{
    public ConsumableType ConsumableType { get; set; }
    public int MaxCount { get; set; }
    public override void FromCsv(string[] values)
    {
        base.FromCsv(values);
        ConsumableType = (ConsumableType)Enum.Parse(typeof(ConsumableType), values[3], true);
        MaxCount = int.Parse(values[4]);
    }
}

public class ItemDataLoader : ILoader<int, ItemData>
{
    public List<WeaponData> weapons = new List<WeaponData>();
    public List<ArmorData> armors = new List<ArmorData>();
    public List<ConsumableData> consumables = new List<ConsumableData>();

    public Dictionary<int, ItemData> MakeDict()
    {
        Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
        foreach (var item in weapons)
        {
            item.ItemType = ItemType.Weapon;
            dict.Add(item.Id, item);
        }
        foreach (var item in armors)
        {
            item.ItemType = ItemType.Armor;
            dict.Add(item.Id, item);
        }
        foreach (var item in consumables)
        {
            item.ItemType = ItemType.Consumable;
            dict.Add(item.Id, item);
        }
        return dict;
    }
}