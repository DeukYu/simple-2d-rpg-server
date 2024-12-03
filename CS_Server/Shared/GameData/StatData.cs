namespace Shared;

[Serializable]
public class StatData : ICsvConvertible
{
    public int Level { get; set; }
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public int Attack { get; set; }
    public float Speed { get; set; }
    public int TotalExp { get; set; }

    public void FromCsv(string[] values)
    {
        Level = int.Parse(values[0]);
        MaxHp = int.Parse(values[1]);
        MaxMp = int.Parse(values[2]);
        Attack = int.Parse(values[3]);
        Speed = float.Parse(values[4]);
        TotalExp = int.Parse(values[5]);
    }
}

public class StatDataLoader : ILoader<int, StatData>
{
    public List<StatData> stats = new List<StatData>();

    public Dictionary<int, StatData> MakeDict()
    {
        Dictionary<int, StatData> dict = new Dictionary<int, StatData>();
        foreach (var stat in stats)
        {
            dict.Add(stat.Level, stat);
        }
        return dict;
    }
}