using Shared;

[Serializable]
public class RewardData : ICsvConvertible
{
    public int MonsterId { get; set; }
    public int ItemId { get; set; }
    public int Probability { get; set; }
    public int Count { get; set; }

    public void FromCsv(string[] values)
    {
        MonsterId = int.Parse(values[0]);
        ItemId = int.Parse(values[1]);
        Probability = int.Parse(values[2]);
        Count = int.Parse(values[3]);
    }
}

public class RewardDataLoader : ILoader<int, List<RewardData>>
{
    public List<RewardData> rewards = new List<RewardData>();
    public Dictionary<int, List<RewardData>> MakeDict()
    {
        Dictionary<int, List<RewardData>> dict = new Dictionary<int, List<RewardData>>();
        foreach (var reward in rewards)
        {
            if (!dict.ContainsKey(reward.MonsterId))
            {
                dict.Add(reward.MonsterId, new List<RewardData>());
            }
            dict[reward.MonsterId].Add(reward);
        }

        return dict;
    }
}