﻿using Google.Protobuf;
using ServerCore;
using System.Net;

namespace DummyClient.Session;

public class ServerSession : PacketSession
{
    public int DummyId { get; set; }
    public void Send(IMessage packet)
    {
        ushort size = (ushort)packet.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];

        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));

        ushort protocolId = PacketManager.Instance.GetMessageId(packet.GetType());
        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));

        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
        Send(new ArraySegment<byte>(sendBuffer));
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Log.Info($"OnConnected: {endPoint}");
    }
    public override void OnDisConnected(EndPoint endPoint)
    {
        Log.Info($"OnDisConnected: {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
    }
}