using AbstractSockets.Abstract;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AbstractSockets.Base
{
    public class ByteStream : AbstractStream<byte[]>
    {
        public ByteStream(NetworkStream networkStream, EndPoint endPoint, bool isServerStream) : base(networkStream, endPoint, isServerStream)
        {

        }

        public override async Task<bool> SendAsync(byte[] data)
        {
            return await SendRawAsync(data);
        }

        protected override void ReceivedRaw(byte[] data)
        {
            RaiseOnReceived(data);
        }
    }
}
