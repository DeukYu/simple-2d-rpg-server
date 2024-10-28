using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Connector
{
    Func<Session> _sessionFactory = null!;
    public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
    {
        Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _sessionFactory = sessionFactory;

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += OnConnectCompleted;
        args.RemoteEndPoint = endPoint;
        args.UserToken = socket;

        RegisterConnect(args);
    }

    void RegisterConnect(SocketAsyncEventArgs args)
    {
        if (args.UserToken is not Socket socket)
        {
            Log.Error("UserToken is not a Socket");
            return;
        }

        bool pending = socket.ConnectAsync(args);
        if (!pending)
            OnConnectCompleted(null, args);
    }

    void OnConnectCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError != SocketError.Success)
        {
            Log.Error($"OnConnectCompleted Failed. {args.SocketError} {args.RemoteEndPoint}");
            return;
        }

        if (args.ConnectSocket == null || args.RemoteEndPoint == null)
        {
            Log.Error("ConnectSocket is null or not connected");
            return;
        }

        Session session = _sessionFactory.Invoke();
        session.Initialize(args.ConnectSocket);
        session.OnConnected(args.RemoteEndPoint);
    }
}
