using System.Collections.Concurrent;

namespace CS_Server;

public class ZoneManager
{
    public static ZoneManager Instance { get; } = new ZoneManager();
    private readonly ConcurrentDictionary<long, Zone> zones = new ConcurrentDictionary<long, Zone>();
    private long _zoneId = 0;
    private object _lock = new object();    

    public Zone? Add(int mapId)
    {
        Zone zone = new Zone();
        zone.Init(mapId);

        lock (_lock)
        {
            zone.ZoneId = _zoneId++;
        }

        return zones.TryAdd(_zoneId, zone) ? zone : null;
    }

    public bool Remove(long zoneId)
    {
        return zones.TryRemove(zoneId, out _);
    }

    public Zone? FindZone(long zoneId)
    {
        return zones.TryGetValue(zoneId, out var zone) ? zone : null;
    }
}
