﻿using Google.Protobuf.Common;
using Google.Protobuf.Enum;
using ServerCore;
using Shared;

namespace CS_Server;

public class Item
{
    public ItemInfo Info { get; } = new ItemInfo();
    public long ItemId
    {
        get { return Info.ItemId; }
        set { Info.ItemId = value; }
    }

    public int TemplateId
    {
        get { return Info.TemplateId; }
        set { Info.TemplateId = value; }
    }
    public int Count
    {
        get { return Info.Count; }
        set { Info.Count = value; }
    }

    public ItemType ItemType { get; private set; }
    public bool Stackable { get; protected set; }
    public Item(ItemType itemType)
    {
        ItemType = itemType;
    }
    public static bool MakeItem(PlayerItemInfo itemInfo, out Item item)
    {
        item = null;

        if (!DataManager.ItemDataDict.TryGetValue(itemInfo.TemplateId, out var itemData))
        {
            Log.Error("Failed to find item data");
            return false;
        }
        switch (itemData.ItemType)
        {
            case ItemType.Weapon:
                item = new Weapon(itemInfo.TemplateId);
                break;
            case ItemType.Armor:
                item =  new Armor(itemInfo.TemplateId);
                break;
            case ItemType.Consumable:
                item = new Consumable(itemInfo.TemplateId);
                break;
        }

        if(item != null)
        {
            item.ItemId = itemInfo.Id;
            item.Count = itemInfo.Count;
        }
        return true;
    }
}

public class Weapon : Item
{
    public WeaponType WeaponType { get; private set; }
    public int Damage { get; private set; }
    public Weapon(int templateId) : base(ItemType.Weapon)
    {
        Init(templateId);
    }
    void Init(int templateId)
    {
        if(!DataManager.ItemDataDict.TryGetValue(templateId, out var itemData))
        {
            Log.Error("Failed to find weapon data");
            return;
        }
        if(itemData.ItemType != ItemType.Weapon)
        {
            Log.Error("Invalid item type");
            return;
        }

        var data = itemData as WeaponData;
        {
            templateId = data.Id;
            Count = 1;
            WeaponType = data.WeaponType;
            Damage = data.Damage;
            Stackable = false;
        }
    }
}

public class Armor : Item
{
    public ArmorType ArmorType { get; private set; }
    public int Defense { get; private set; }
    public Armor(int templateId) : base(ItemType.Armor)
    {
        Init(templateId);
    }
    void Init(int templateId)
    {
        if (!DataManager.ItemDataDict.TryGetValue(templateId, out var itemData))
        {
            Log.Error("Failed to find armor data");
            return;
        }
        if (itemData.ItemType != ItemType.Armor)
        {
            Log.Error("Invalid item type");
            return;
        }
        var data = itemData as ArmorData;
        {
            templateId = data.Id;
            Count = 1;
            ArmorType = data.ArmorType;
            Defense = data.Defense;
            Stackable = false;
        }
    }
}

public class Consumable : Item
{
    public ConsumeType ConsumeType { get; private set; }
    public int MaxCount { get; private set; }
    public Consumable(int templateId) : base(ItemType.Consumable)
    {
        Init(templateId);
    }
    void Init(int templateId)
    {
        if (!DataManager.ItemDataDict.TryGetValue(templateId, out var itemData))
        {
            Log.Error("Failed to find consumable data");
            return;
        }
        if (itemData.ItemType != ItemType.Consumable)
        {
            Log.Error("Invalid item type");
            return;
        }
        var data = itemData as ConsumableData;
        {
            templateId = data.Id;
            Count = 1;
            MaxCount = data.MaxCount;
            ConsumeType = data.ConsumeType;
            Stackable = true;
        }
    }
}