using ServerCore;
using Shared.DB;

namespace CS_Server;

public partial class DbTransaction : JobSerializer
{
    public static DbTransaction Instance { get; } = new DbTransaction();

    public static void UpdatePlayerStatus(Player player, Zone zone)
    {
        if (player == null || zone == null)
        {
            return;
        }

        var playerInfo = new PlayerInfo();
        playerInfo.Id = player.PlayerUid;
        playerInfo.Hp = player.StatInfo.Hp;
        playerInfo.Mp = player.StatInfo.Mp;

        Instance.ScheduleJob(() =>
        {
            using (AccountDB db = new AccountDB())
            {
                db.SetModifiedProperties(playerInfo, nameof(playerInfo.Hp), nameof(playerInfo.Mp));
                if (db.SaveChangesEx() == false)
                {
                    Log.Error("Failed to save player stat info");
                    return;
                }
            }
        });
    }
    public static void RewardPlayer(Player player, RewardData rewardData, Zone zone)
    {
        if(player == null || rewardData == null || zone == null)
        {
            return;
        }

        // TODO : 문제 발생
        int? slot = player.Inven.GetEmptySlot();
        if (slot == null)
            return;

        PlayerItemInfo playerItemInfo = new PlayerItemInfo
        {
            ItemId = rewardData.ItemId,
            Count = rewardData.Count,
            Slot = slot.Value,
            PlayerId = player.PlayerUid

        };

        Instance.ScheduleJob(() =>
        {
            using (AccountDB db = new AccountDB())
            {
                db.ItemInfo.Add(playerItemInfo);
                if (db.SaveChangesEx() == false)
                {
                    Log.Error("Failed to save player item info");
                    return;
                }
                zone.ScheduleJob(() =>
                {
                    if (Item.MakeItem(playerItemInfo, out var newItem) == false)
                    {
                        Log.Error("Failed to make item");
                        return;
                    }
                    player.Inven.Add(newItem);
                    player.SendAddItemPacket(newItem.Info);
                });
            }
        });
    }
}
