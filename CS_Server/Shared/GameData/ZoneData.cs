namespace Shared;

[Serializable]
public class ZoneData : ICsvConvertible
{
    public int MapId { get; set; }
    public int ZoneId { get; set; }

    public void FromCsv(string[] values)
    {
        MapId = int.Parse(values[0]);
        ZoneId = int.Parse(values[1]);
    }
}

public class ZoneDataLoader : ILoader<int, ZoneData>
{
    public List<ZoneData> zones = new List<ZoneData>();

    public Dictionary<int, ZoneData> MakeDict()
    {
        Dictionary<int, ZoneData> dict = new Dictionary<int, ZoneData>();
        foreach (var zone in zones)
        {
            dict.Add(zone.MapId, zone);
        }
        return dict;
    }
}