using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;

namespace AbstractSockets.Base.Aes
{
    public class AesServer : AbstractServer<byte[]>
    {
        protected override IAbstractStream<byte[]> CreateStream(NetworkStream ns, EndPoint ep)
        {
            return new AesStream(ns, ep, true);
        }
    }
}
