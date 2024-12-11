using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public partial class Zone : JobSerializer
{
    public void HandleEquipItem(Player player, int itemId, bool equipped)
    {
        if(player == null)
        {
            Log.Error("HandleEquipItem : player is null");
            return;
        }

        var item = player.Inven.Get(itemId);
        if (item == null)
        {
            Log.Error("HandleEquipItem : item is null");
            return;
        }

        // 메모리 선적용
        item.Equipped = equipped;

        // DB 적용
        DbTransaction.EquipItemNotify(player, item);

        // 클라 전송
        S2C_EquipItem equipItemPacket = new S2C_EquipItem();
        equipItemPacket.ItemId = itemId;
        equipItemPacket.Equipped = equipped;
        player.Session.Send(equipItemPacket);
    }
}
