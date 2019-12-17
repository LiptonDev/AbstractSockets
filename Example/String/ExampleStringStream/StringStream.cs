using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExampleStringStream
{
    public class StringStream : AbstractStream<string>
    {
        public StringStream(NetworkStream ns, EndPoint ep, bool isServerStream) : base(ns, ep, isServerStream)
        {

        }

        public override async Task<bool> SendAsync(string data)
        {
            return await SendRawAsync(Encoding.UTF8.GetBytes(data));
        }

        protected override void ReceivedRaw(byte[] data)
        {
            RaiseOnReceived(Encoding.UTF8.GetString(data));
        }
    }
}
