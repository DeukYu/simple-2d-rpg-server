using Google.Protobuf.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

[Serializable]
public class RewardData
{
    public int Probability { get; set; }
    public int ItemId { get; set; }
    public int Count { get; set; }
}

[Serializable]
public class MonsterData : ICsvConvertible
{
    public int Id { get; set; }
    public string Name { get; set; }
    public StatInfo Stat { get; set; }
    public List<RewardData> Rewards { get; set; }
    //public string prefabPath { get; set; }    // 클라에서는 필요

    public void FromCsv(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
        Stat = new StatInfo();
    }
}

public class MonsterDataLoader : ILoader<int, MonsterData>
{
    public List<MonsterData> monsters = new List<MonsterData>();

    public Dictionary<int, MonsterData> MakeDict()
    {
        Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
        foreach (var monster in monsters)
        {
            dict.Add(monster.Id, monster);
        }
        return dict;
    }
}