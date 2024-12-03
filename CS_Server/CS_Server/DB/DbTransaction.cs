using Microsoft.EntityFrameworkCore;
using ServerCore;
using Shared;

namespace CS_Server;

public class DbTransaction : JobSerializer
{
    public static DbTransaction Instance { get; } = new DbTransaction();

    public static void UpdatePlayerStatus(Player player, Zone zone)
    {
        if (player == null || zone == null)
        {
            return;
        }

        var playerStatInfo = new PlayerStatInfo();
        playerStatInfo.PlayerId = player.PlayerId;
        playerStatInfo.Hp = player.StatInfo.Hp;
        playerStatInfo.Mp = player.StatInfo.Mp;

        Instance.Push(() =>
        {
            using (AccountDB db = new AccountDB())
            {
                db.SetModifiedProperties(playerStatInfo, nameof(playerStatInfo.Hp), nameof(playerStatInfo.Mp));
                if (db.SaveChangesEx() == false)
                {
                    Log.Error("Failed to save player stat info");
                    return;
                }
                zone.Push(() =>
                {
                    Log.Info($"UpdatePlayerStatus: PlayerId: {playerStatInfo.PlayerId}, Hp: {playerStatInfo.Hp}, Mp: {playerStatInfo.Mp}");
                });
            }
        });
    }
}
