using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Initialize(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            receiveArgs.SetBuffer(new byte[1024], 0, 1024);

            RegisterReceive(receiveArgs);
        }

        public void Send(byte[] sendBuff)
        {
           _socket.Send(sendBuff);
        }

        public void Disconnect()
        {
            // 이미 끊겼다면 return (두번의 Disconnect를 방지)
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region Network communication
        private void RegisterReceive(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if(pending == false)
            {
                OnReceiveCompleted(null, args);
            }
        }
        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");
                    RegisterReceive(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnReceiveCompleted Failed {e}");
                }
            }
            else
            {
                Console.WriteLine($"OnReceiveCompleted Failed {args.SocketError}");
            }
        }
        #endregion
    }
}
