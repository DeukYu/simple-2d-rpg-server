using System.Collections.Concurrent;

namespace CS_Server;

class ZoneManager
{
    public static ZoneManager Instance { get; } = new ZoneManager();
    private readonly ConcurrentDictionary<long, Zone> zones = new ConcurrentDictionary<long, Zone>();
    private long _zoneId = 1;

    public Zone? FindZone(long zoneId)
    {
        if (zones.TryGetValue(zoneId, out Zone? zone))
            return zone;
        return null;
    }
}
