using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AbstractSockets.Base.String
{
    public class StringServer : AbstractServer<string>
    {
        Encoding encoding;

        public StringServer(Encoding encoding)
        {
            this.encoding = encoding;
        }

        protected override IAbstractStream<string> CreateStream(NetworkStream ns, EndPoint ep)
        {
            return new StringStream(encoding, ns, ep, true);
        }
    }
}
