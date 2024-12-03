namespace Shared;

[Serializable]
public class ProjectileInfoData : ICsvConvertible
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Speed { get; set; }
    public int Range { get; set; }
    public string Prefab { get; set; }

    public void FromCsv(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
        Speed = float.Parse(values[2]);
        Range = int.Parse(values[3]);
        Prefab = values[4];
    }
}

public class ProjectileDataLoader : ILoader<int, ProjectileInfoData>
{
    public List<ProjectileInfoData> projectileInfos = new List<ProjectileInfoData>();

    public Dictionary<int, ProjectileInfoData> MakeDict()
    {
        Dictionary<int, ProjectileInfoData> dict = new Dictionary<int, ProjectileInfoData>();
        foreach (var projectile in projectileInfos)
        {
            dict.Add(projectile.Id, projectile);
        }
        return dict;
    }
}
