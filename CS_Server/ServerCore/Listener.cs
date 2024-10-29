using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        private Socket _listenSocket = null!;
        private Func<Session> _sessionFactory = null!;

        public void Initialize(IPEndPoint endPoint, Func<Session> sessionFactory, int backLogCount = 10)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(backLogCount);

            AcceptLoop();
        }

        private void AcceptLoop()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            RegisterAcceptAsync(args);
        }

        void RegisterAcceptAsync(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket?.AcceptAsync(args) ?? false;
            if (!pending)
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                Log.Error(args.SocketError.ToString());
                return;
            }

            if (args.AcceptSocket == null || args.AcceptSocket.RemoteEndPoint == null)
            {
                Log.Error("AcceptSocket is null or not connected");
                return;
            }

            Session session = _sessionFactory.Invoke();
            session.Initialize(args.AcceptSocket);
            session.OnConnected(args.AcceptSocket.RemoteEndPoint);

            RegisterAcceptAsync(args);
        }

        public Socket Accept()
        {
            return _listenSocket.Accept();
        }
    }
}
