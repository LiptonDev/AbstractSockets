using System.Net;
using System.Net.Sockets;

namespace AbstractSockets.Abstract
{
    public abstract class StreamCreator<T>
    {
        protected abstract IAbstractStream<T> CreateStream(NetworkStream ns, EndPoint ep);
    }
}
