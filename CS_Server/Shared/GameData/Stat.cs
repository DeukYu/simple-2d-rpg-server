namespace Shared;

[Serializable]
public class Stat : ICsvConvertible
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

public class StatData : ILoader<int, Stat>
{
    public List<Stat> stats = new List<Stat>();

    public Dictionary<int, Stat> MakeDict()
    {
        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
        foreach (var stat in stats)
        {
            dict.Add(stat.Level, stat);
        }
        return dict;
    }
}