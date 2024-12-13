using System.Collections.Concurrent;

namespace CS_Server;

public class GameLogic : JobSerializer
{
    public static GameLogic Instance { get; } = new GameLogic();
    private readonly ConcurrentDictionary<long, Zone> zones = new ConcurrentDictionary<long, Zone>();
    private long _zoneId = 0;

    public void Update()
    {
        Flush();

        foreach (var zone in zones.Values)
        {
            zone.Update();
        }
    }

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
