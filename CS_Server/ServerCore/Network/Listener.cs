using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        private Socket _listenSocket = null!;
        private Func<Session> _sessionFactory = null!;

        public void Initialize(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backLog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(backLog);

            AcceptLoop(register);
        }

        private void AcceptLoop(int register)
        {
            for (int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnAcceptCompleted;
                RegisterAcceptAsync(args);
            }
        }

        void RegisterAcceptAsync(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            try
            {
                bool pending = _listenSocket?.AcceptAsync(args) ?? false;
                if (pending == false)
                    OnAcceptCompleted(null, args);
            }
            catch(Exception e)
            {
                Log.Error($"AcceptAsync Failed {e}");
            }
        }

        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            try
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
            }
            catch(Exception e)
            {
                Log.Error($"OnAcceptCompleted Failed {e}");
            }
            RegisterAcceptAsync(args);
        }

        public Socket Accept()
        {
            return _listenSocket.Accept();
        }
    }
}
