﻿using ServerCore;
using Shared;

namespace CS_Server;

public class DataManager
{
    public static Dictionary<int, StatData> StatDict { get; private set; } = new Dictionary<int, StatData>();
    public static Dictionary<int, SkillData> SkillDict { get; private set; } = new Dictionary<int, SkillData>();
    public static Dictionary<int, ProjectileInfoData> ProjectileInfoDict { get; private set; } = new Dictionary<int, ProjectileInfoData>();
    public static Dictionary<int, ItemData> ItemDataDict { get; private set; } = new Dictionary<int, ItemData>();
    public static Dictionary<int, MonsterData> MonterDataDict { get; private set; } = new Dictionary<int, MonsterData>();
    public static void LoadData()
    {
        StatDict = DataLoader.Load<StatDataLoader, int, StatData>("StatData");
        SkillDict = DataLoader.Load<SkillDataLoader, int, SkillData>("SkillData");
        ProjectileInfoDict = DataLoader.Load<ProjectileDataLoader, int, ProjectileInfoData>("ProjectileInfoData");
        ItemDataDict = DataLoader.Load<ItemDataLoader, int, ItemData>("ItemData");
        MonterDataDict = DataLoader.Load<MonsterDataLoader, int, MonsterData>("MonsterData");

        Log.Info("Data loaded");
    }
}