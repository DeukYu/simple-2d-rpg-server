using System.Collections.Concurrent;

namespace CS_Server;

class ZoneManager
{
    private readonly ConcurrentDictionary<long, Zone> zoneDict = new ConcurrentDictionary<long, Zone>();

    public Zone? FindZone(long zoneId)
    {
        if (zoneDict.TryGetValue(zoneId, out Zone? zone))
            return zone;
        return null;
    }
}
