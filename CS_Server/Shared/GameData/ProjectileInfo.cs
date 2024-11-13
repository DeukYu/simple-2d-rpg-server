namespace Shared;

[Serializable]
public class ProjectileInfo : ICsvConvertible
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

public class ProjectileData : ILoader<int, ProjectileInfo>
{
    public List<ProjectileInfo> projectileInfos = new List<ProjectileInfo>();

    public Dictionary<int, ProjectileInfo> MakeDict()
    {
        Dictionary<int, ProjectileInfo> dict = new Dictionary<int, ProjectileInfo>();
        foreach (var projectile in projectileInfos)
        {
            dict.Add(projectile.Id, projectile);
        }
        return dict;
    }
}
