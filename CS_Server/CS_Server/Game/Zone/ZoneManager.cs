using System.Collections.Concurrent;

namespace CS_Server;

public class ZoneManager
{
    public static ZoneManager Instance { get; } = new ZoneManager();
    private readonly ConcurrentDictionary<long, Zone> zones = new ConcurrentDictionary<long, Zone>();
    private long _zoneId = 0;

    public Zone? Add(int mapId)
    {
        Zone zone = new Zone();
        zone.Push(zone.Init, mapId);

        long newZoneId = Interlocked.Increment(ref _zoneId);
        zone.ZoneId = newZoneId;

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
