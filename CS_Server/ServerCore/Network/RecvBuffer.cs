using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore;

public class RecvBuffer
{
    private readonly ArraySegment<byte> _buffer;
    private int _readPos;
    private int _writePos;

    public RecvBuffer(int bufferSize)
    {
        _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
    }

    public int DataSize { get { return _writePos - _readPos; } }
    public int FreeSize { get { return _buffer.Count - _writePos; } }

    private byte[] GetBufferArray()
    {
        if (_buffer.Array == null)
            throw new NullReferenceException("Buffer.Array is null");
        return _buffer.Array;
    }

    public ArraySegment<byte> ReadSegment
    {
        get
        {
            return new ArraySegment<byte>(GetBufferArray(), _buffer.Offset + _readPos, DataSize);
        }
    }
    public ArraySegment<byte> WriteSegment
    {
        get
        {
            return new ArraySegment<byte>(GetBufferArray(), _buffer.Offset + _writePos, FreeSize);
        }
    }

    public void Clean()
    {
        int dataSize = DataSize;
        if (dataSize == 0)
        {
            // 남은 데이터가 없을 경우 복사하지 않고 커서 위치만 리셋
            _readPos = _writePos = 0;
        }
        else
        {
            // 남은 데이터가 있을 경우 데이터를 앞쪽으로 복사
            var bufferArray = GetBufferArray();
            Array.Copy(bufferArray, _buffer.Offset + _readPos, bufferArray, _buffer.Offset, dataSize);
            _readPos = 0;
            _writePos = dataSize;
        }
    }

    public bool OnRead(int numOfBytes)
    {
        if (numOfBytes > DataSize)
        {
            Log.Error($"OnRead Failed: numOfBytes({numOfBytes}) > DataSize({DataSize})");
            return false;
        }

        _readPos += numOfBytes;
        return true;
    }
    public bool OnWrite(int numOfBytes)
    {
        if (numOfBytes > FreeSize)
        {
            Log.Error($"OnWrite Failed: numOfBytes({numOfBytes}) > FreeSize({FreeSize})");
            return false;
        }

        _writePos += numOfBytes;
        return true;
    }
}
