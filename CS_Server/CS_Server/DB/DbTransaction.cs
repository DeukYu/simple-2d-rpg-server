using Google.Protobuf.Common;
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
            TemplateId = rewardData.ItemId,
            Count = rewardData.Count,
            Slot = slot.Value,
            PlayerId = player.PlayerId

        };

        Instance.Push(() =>
        {
            using (AccountDB db = new AccountDB())
            {
                db.ItemInfo.Add(playerItemInfo);
                if (db.SaveChangesEx() == false)
                {
                    Log.Error("Failed to save player item info");
                    return;
                }
                zone.Push(() =>
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
