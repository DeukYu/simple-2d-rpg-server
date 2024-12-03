using Google.Protobuf.Enum;

namespace Shared;

[Serializable]
public class SkillData : ICsvConvertible
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Cooltime { get; set; }
    public int Damage { get; set; }
    public SkillType SkillType { get; set; }
    public int ProjectileId { get; set; }

    public void FromCsv(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
        Cooltime = float.Parse(values[2]);
        Damage = int.Parse(values[3]);
        SkillType = (SkillType)Enum.Parse(typeof(SkillType), values[4], true);
        ProjectileId = int.Parse(values[5]);
    }
}

public class SkillDataLoader : ILoader<int, SkillData>
{
    public List<SkillData> skills = new List<SkillData>();

    public Dictionary<int, SkillData> MakeDict()
    {
        Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
        foreach (var stat in skills)
        {
            dict.Add(stat.Id, stat);
        }
        return dict;
    }
}