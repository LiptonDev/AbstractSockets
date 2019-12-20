using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AbstractSockets.Base.String
{
    public class StringStream : AbstractStream<string>
    {
        Encoding encoding;

        public StringStream(Encoding encoding, NetworkStream ns, EndPoint ep, bool isServerStream) : base(ns, ep, isServerStream)
        {
            this.encoding = encoding;
        }

        public async override Task<bool> SendAsync(string data)
        {
            return await SendRawAsync(encoding.GetBytes(data));
        }

        protected override string ReceivedRaw(byte[] data)
        {
            return encoding.GetString(data);
        }
    }
}
