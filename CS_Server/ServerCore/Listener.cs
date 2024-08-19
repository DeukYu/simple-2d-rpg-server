using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Listener
    {
        Socket? _listenSocket;
        Action<Socket>? _onAceeptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAceeptHandler += onAcceptHandler;

            _listenSocket?.Bind(endPoint);

            _listenSocket?.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args, Get_listenSocket());
        }

        private Socket? Get_listenSocket()
        {
            return _listenSocket;
        }

        void RegisterAccept(SocketAsyncEventArgs args, Socket? _listenSocket)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                _onAceeptHandler?.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args, Get_listenSocket());
        }

        public Socket Accept()
        {
            

            return _listenSocket.Accept();
        }
    }
}
