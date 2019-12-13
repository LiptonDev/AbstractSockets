using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;

namespace AbstractSockets.Base
{
    public class ByteStream : AbstractStream<byte[]>
    {
        public ByteStream(NetworkStream networkStream, EndPoint endPoint) : base(networkStream, endPoint)
        {

        }

        public override bool Send(byte[] data)
        {
            return SendRaw(data);
        }

        protected override void ReceivedRaw(byte[] data)
        {
            RaiseOnReceived(data);
        }
    }
}
