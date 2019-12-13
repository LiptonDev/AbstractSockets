using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSockets.Abstract
{
    class TCPBuffer
    {
        byte[] buffer;

        public TCPBuffer()
        {
            buffer = new byte[0];
        }

        public byte[] GetPayload(byte[] data)
        {
            byte[] length = BitConverter.GetBytes(data.Length);
            byte[] payload = new byte[data.Length + 4];

            Buffer.BlockCopy(length, 0, payload, 0, 4);
            Buffer.BlockCopy(data, 0, payload, 4, data.Length);

            return payload;
        }

        public IEnumerable<byte[]> BufferProcessing(byte[] data)
        {
            int newLength = buffer.Length + data.Length;
            byte[] temp = new byte[newLength];
            Buffer.BlockCopy(buffer, 0, temp, 0, buffer.Length);
            Buffer.BlockCopy(data, 0, temp, buffer.Length, data.Length);
            buffer = temp;

            while (buffer.Length >= 4)
            {
                int packetSize = BitConverter.ToInt32(buffer, 0);

                if (buffer.Length >= 4 + packetSize && packetSize > 0)
                {
                    byte[] payload = new byte[packetSize];
                    Buffer.BlockCopy(buffer, 0, payload, 0, packetSize);

                    int newSize = buffer.Length - 4 - packetSize;
                    temp = new byte[newSize];
                    int offset = 4 + packetSize;
                    Buffer.BlockCopy(buffer, offset, temp, 0, newSize);
                    buffer = temp;

                    yield return payload;
                }
                else yield break;
            }
        }
    }
}
