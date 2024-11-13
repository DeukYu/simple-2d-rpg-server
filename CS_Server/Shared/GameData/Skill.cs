using Google.Protobuf.Enum;

namespace Shared;

[Serializable]
public class Skill : ICsvConvertible
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

public class SkillData : ILoader<int, Skill>
{
    public List<Skill> skills = new List<Skill>();

    public Dictionary<int, Skill> MakeDict()
    {
        Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
        foreach (var stat in skills)
        {
            dict.Add(stat.Id, stat);
        }
        return dict;
    }
}