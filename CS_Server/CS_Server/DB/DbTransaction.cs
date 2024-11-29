using Microsoft.EntityFrameworkCore;
using ServerCore;
using Shared;

namespace CS_Server;

public class DbTransaction : JobSerializer
{
    public static DbTransaction Instance { get; } = new DbTransaction();

    public static void SavePlayerStatus_AllInOne(Player player, Zone zone)
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

                db.Entry(playerStatInfo).State = EntityState.Unchanged;
                db.Entry(playerStatInfo).Property(nameof(playerStatInfo.Hp)).IsModified = true;
                db.Entry(playerStatInfo).Property(nameof(playerStatInfo.Mp)).IsModified = true;
                if (db.SaveChangesEx() == false)
                {
                    Log.Error("Failed to save player stat info");
                    return;
                }

                zone.Push(() =>
                {
                    Log.Info($"SavePlayerStatus_AllInOne: PlayerId: {playerStatInfo.PlayerId}, Hp: {playerStatInfo.Hp}, Mp: {playerStatInfo.Mp}");
                });
            }
        });
    }
}
