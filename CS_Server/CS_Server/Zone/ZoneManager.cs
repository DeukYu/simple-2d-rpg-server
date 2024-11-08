using NLog.Common;
using NLog.LayoutRenderers;
using System.Collections.Concurrent;

namespace CS_Server;

public class ZoneManager
{
    public static ZoneManager Instance { get; } = new ZoneManager();
    private readonly ConcurrentDictionary<long, Zone> zones = new ConcurrentDictionary<long, Zone>();
    private long _zoneId = 1;
    private object _lock = new object();    

    public Zone? Add()
    {
        Zone zone = new Zone();

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
