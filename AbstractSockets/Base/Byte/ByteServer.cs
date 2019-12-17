using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;

namespace AbstractSockets.Base
{
    public class ByteServer : AbstractServer<byte[]>
    {
        public ByteServer()
        {
        }

        protected override IAbstractStream<byte[]> CreateStream(NetworkStream ns, EndPoint ep)
        {
            return new ByteStream(ns, ep, true);
        }
    }
}
