using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AbstractSockets.Base.String
{
    public class StringClient : AbstractClient<string>
    {
        Encoding encoding;

        public StringClient(Encoding encoding)
        {
            this.encoding = encoding;
        }

        protected override IAbstractStream<string> CreateStream(NetworkStream ns, EndPoint ep)
        {
            return new StringStream(encoding, ns, ep, false);
        }
    }
}
