namespace Shared;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
