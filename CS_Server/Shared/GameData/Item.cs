using Google.Protobuf.Enum;

namespace Shared;

[Serializable]
public class ItemData : ICsvConvertible
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ItemType ItemType { get; set; }

    public void FromCsv(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
        ItemType = (ItemType)Enum.Parse(typeof(ItemType), values[2], true);
    }
}

public class WeaponData : ItemData
{
    public WeaponType WeaponType { get; set; }
    public int Damage { get; set; }
    public new void FromCsv(string[] values)
    {
        base.FromCsv(values);
        Damage = int.Parse(values[3]);
    }
}

public class ArmorData : ItemData
{
    public ArmorType ArmorType { get; set; }
    public int Defense { get; set; }
    public new void FromCsv(string[] values)
    {
        base.FromCsv(values);
        Defense = int.Parse(values[3]);
    }
}

public class ConsumableData : ItemData
{
    public ConsumeType ConsumeType { get; set; }
    public int MaxCount { get; set; }
    public new void FromCsv(string[] values)
    {
        base.FromCsv(values);
        MaxCount = int.Parse(values[3]);
    }
}

public class ItemDataLoader : ILoader<int, ItemData>
{
    public List<WeaponData> weapons = new List<WeaponData>();
    public List<ArmorData> armors = new List<ArmorData>();
    public List<ConsumableData> consumes = new List<ConsumableData>();

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
        foreach (var item in consumes)
        {
            item.ItemType = ItemType.Consumable;
            dict.Add(item.Id, item);
        }
        return dict;
    }
}