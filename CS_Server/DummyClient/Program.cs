﻿using DummyClient.Session;
using ServerCore;
using System.Net;

namespace DummyClient;
class Program
{
    static int DummyClientCount { get; } = 1;

    static void Main(string[] args)
    {
        Thread.Sleep(3000);

        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 8000);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, DummyClientCount);

        while(true)
        {
            Thread.Sleep(1000);
        }
    }
}