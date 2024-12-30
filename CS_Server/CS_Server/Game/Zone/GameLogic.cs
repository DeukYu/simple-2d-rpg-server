using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

public class GameLogic : JobSerializer
{
    public static GameLogic Instance { get; } = new GameLogic();
    private readonly ConcurrentDictionary<int, Zone> zones = new ConcurrentDictionary<int, Zone>();

    public void Update()
    {
        ProcessJobs();

        foreach (var zone in zones.Values)
        {
            zone.Update();
        }
    }

    public void Init()
    {
        // 맵 데이터를 읽어서 Zone을 생성
        var zoneDatas = DataManager.ZoneDict.Values;
        foreach (var mapData in zoneDatas)
        {
            if(!TryAdd(mapData.ZoneId, mapData.MapId, out var _))
            {
                Log.Error($"Failed to add zone. ZoneId: {mapData.ZoneId} MapId: {mapData.MapId}");
            }
        }
    }

    public bool TryAdd(int zoneId, int mapId, out Zone zone)
    {
        zone = new Zone();
        zone.ScheduleJob(zone.Init, mapId, 10);

        return zones.TryAdd(zoneId, zone);
    }

    public bool Remove(int zoneId)
    {
        return zones.TryRemove(zoneId, out _);
    }

    public Zone? Find(int zoneId)
    {
        return zones.TryGetValue(zoneId, out var zone) ? zone : null;
    }
}
