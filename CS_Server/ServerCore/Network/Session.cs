using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public abstract class PacketSession : Session
{
    public static readonly int HeaderSize = 2;
    public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    public sealed override int OnRecv(ArraySegment<byte> buffer)
    {
        int processLen = 0;
        int packetCount = 0;

        while (true)
        {
            if (buffer.Count < HeaderSize)
                break;

            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            if (buffer.Count < dataSize)
                break;

            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
            processLen += dataSize;
            packetCount++;

            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }

        if (packetCount > 1)
            Log.Info($"Packet Count: {packetCount}");

        return processLen;
    }
}
public abstract class Session
{
    private Socket _socket = null!;
    private int _disconnected = 0;

    RecvBuffer _recvBuffer = new RecvBuffer(65535);

    private object _lock = new object();
    private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
    private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
    private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
    private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

    public abstract void OnConnected(EndPoint endPoint);
    public abstract void OnDisConnected(EndPoint endPoint);
    public abstract int OnRecv(ArraySegment<byte> buffer);
    public abstract void OnSend(int numOfBytes);

    void Clear()
    {
        lock (_lock)
        {
            _sendQueue.Clear();
            _pendingList.Clear();
        }
    }
    public void Initialize(Socket socket)
    {
        _socket = socket;

        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

        RegisterRecvAsync();
    }

    public void Send(List<ArraySegment<byte>> sendBuffList)
    {
        if (sendBuffList.Count == 0)
            return;

        lock (_lock)
        {
            foreach (ArraySegment<byte> sendBuff in sendBuffList)
                _sendQueue.Enqueue(sendBuff);

            if (_pendingList.Count == 0)
                RegisterSendAsync();
        }
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        lock (_lock)
        {
            _sendQueue.Enqueue(sendBuff);
            if (_pendingList.Count == 0)
                RegisterSendAsync();
        }
    }

    public void Disconnect()
    {
        // 이미 끊겼다면 return (두번의 Disconnect를 방지)
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;

        if (_socket.RemoteEndPoint == null)
        {
            Log.Error("Disconnect RemoteEndPoint is null");
            return;
        }

        OnDisConnected(_socket.RemoteEndPoint);
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        Clear();
    }

    #region Network communication
    private void RegisterSendAsync()
    {
        if (_disconnected == 1)
            return;

        while (_sendQueue.Count > 0)
        {
            var buff = _sendQueue.Dequeue();
            _pendingList.Add(buff);
        }
        _sendArgs.BufferList = _pendingList;

        try
        {
            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }
        catch (Exception e)
        {
            Log.Error($"RegisterSendAsync Failed {e}");
        }   
    }
    private void OnSendCompleted(object? sender, SocketAsyncEventArgs args)
    {
        lock (_lock)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    OnSend(_sendArgs.BytesTransferred);

                    if (_sendQueue.Count > 0)
                    {
                        RegisterSendAsync();
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"OnSendCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
    }
    private void RegisterRecvAsync()
    {
        if(_disconnected == 1)
            return;

        _recvBuffer.Clean();
        var segment = _recvBuffer.WriteSegment;
        _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

        try
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
            {
                OnRecvCompleted(null, _recvArgs);
            }
        }
        catch(Exception e)
        {
            Log.Error($"RegisterRecvAsync Failed {e}");
        }
        
    }
    private void OnRecvCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            try
            {
                // Write 커서 이동
                if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                {
                    Disconnect();
                    return;
                }

                // 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받아옴
                int processLen = OnRecv(_recvBuffer.ReadSegment);
                if (processLen < 0 || _recvBuffer.DataSize < processLen)
                {
                    Disconnect();
                    return;
                }

                // Read 커서 이동
                if (_recvBuffer.OnRead(processLen) == false)
                {
                    Disconnect();
                    return;
                }

                RegisterRecvAsync();
            }
            catch (Exception e)
            {
                Log.Error($"OnReceiveCompleted Failed {e}");
            }
        }
        else
        {
            Disconnect();
        }
    }
    #endregion
}
