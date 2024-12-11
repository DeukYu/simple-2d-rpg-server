using Google.Protobuf.Common;
using Microsoft.EntityFrameworkCore;
using ServerCore;
using Shared;

namespace CS_Server;

public partial class DbTransaction : JobSerializer
{
    public static void EquipItemNotify(Player player, Item item)
    {
        if(player == null || item == null)
        {
            Log.Error("EquipItemNotify: player or item is null");
            return;
        }

        var itemInfo = new PlayerItemInfo
        {
            Id = item.ItemId,
            Equipped = item.Equipped,
        };

        Instance.Push(() =>
        {
            using(var accountDB = new AccountDB())
            {
                accountDB.Entry(itemInfo).State = EntityState.Unchanged;
                accountDB.Entry(itemInfo).Property(x => x.Equipped).IsModified = true;

                if(accountDB.SaveChangesEx() == false)
                {
                    Log.Error("EquipItemNotify: Failed to save changes to the database.");
                    return;
                }
            }
        });
    }
}
