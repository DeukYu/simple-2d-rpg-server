using Google.Protobuf.Common;

namespace Shared;

[Serializable]
public class MonsterData : ICsvConvertible
{
    public int Id { get; set; }
    public string Name { get; set; }
    public StatInfo Stat { get; set; }
    //public string prefabPath { get; set; }    // 클라에서는 필요

    public void FromCsv(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
        Stat = new StatInfo
        {
            Level = int.Parse(values[2]),

            Hp = int.Parse(values[3]),
            MaxHp = int.Parse(values[3]),

            Mp = int.Parse(values[4]),
            MaxMp = int.Parse(values[4]),

            Attack = int.Parse(values[5]),
            Speed = float.Parse(values[6]),

            Exp = 0,
            TotalExp = int.Parse(values[7]),
        };
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