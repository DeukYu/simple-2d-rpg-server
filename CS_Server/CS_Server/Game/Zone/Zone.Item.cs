﻿using Google.Protobuf.Enum;
using Google.Protobuf.Protocol;
using ServerCore;

namespace CS_Server;

public partial class Zone : JobSerializer
{
    public void HandleEquipItem(Player player, int itemId, bool equipped)
    {
        if (player == null)
        {
            Log.Error("HandleEquipItem : player is null");
            return;
        }

        player.HandleEquipItem(itemId, equipped);
    }
}
