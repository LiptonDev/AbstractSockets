using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;

namespace AbstractSockets.Base.Byte
{
    public class ByteClient : AbstractClient<byte[]>
    {
        public ByteClient()
        {
        }

        protected override IAbstractStream<byte[]> CreateStream(NetworkStream ns, EndPoint ep)
        {
            return new ByteStream(ns, ep, false);
        }
    }
}
