using AbstractSockets.Delegates;
using AbstractSockets.Enums;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AbstractSockets.Abstract
{
    public abstract class AbstractClient<T> : StreamCreator<T>, IAbstractClient<T>
    {
        #region Events
        /// <summary>
        /// Raised when the connection is established succesfully.
        /// </summary>
        public event ClientOnConnected<T> OnConnected;

        /// <summary>
        /// Raised when the connection is closed by the client or server.
        /// </summary>
        public event ClientOnDisconnected<T> OnDisconnected;

        /// <summary>
        /// Raised when data has been received.
        /// </summary>
        public event ClientOnReceived<T> OnReceived;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the remote host name.
        /// </summary>
        public string RemoteHost { get; private set; }

        /// <summary>
        /// Gets the remote port number.
        /// </summary>
        public int RemotePort { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the client is connected.
        /// </summary>
        public bool IsConnected { get; private set; }
        #endregion

        #region Private fields
        /// <summary>
        /// Client stream.
        /// </summary>
        IAbstractStream<T> stream;
        #endregion

        #region Connect region
        /// <summary>
        /// Connects to the server on the specified host and port.
        /// </summary>
        /// <param name="host">The remote host name.</param>
        /// <param name="port">The remote port number.</param>
        public async Task ConnectAsync(string host, int port)
        {
            if (IsConnected)
                return;

            RemoteHost = host;
            RemotePort = port;

            var client = new TcpClient();
            await client.ConnectAsync(host, port);

            IsConnected = true;

            var ns = client.GetStream();
            var ep = client.Client.RemoteEndPoint;

            stream = CreateStream(ns, ep);
            stream.OnStarted += Stream_OnStarted;
            stream.OnStopped += Stream_OnStopped;
            stream.OnReceived += Stream_OnReceived;

            stream.Start();
        }

        /// <summary>
        /// Connects to the server, returns a bool indicating whether the attempt failed or succeeded.
        /// </summary>
        /// <param name="host">The remote host name.</param>
        /// <param name="port">The remote port number.</param>
        /// <returns></returns>
        public async Task<bool> TryConnectAsync(string host, int port)
        {
            try
            {
                await ConnectAsync(host, port);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Stream events
        private void Stream_OnReceived(IAbstractStream<T> stream, T data)
        {
            OnReceived?.Invoke(this, data);
        }

        private void Stream_OnStopped(IAbstractStream<T> stream, NetStoppedReason reason)
        {
            Disconnect(reason);
        }

        private void Stream_OnStarted(IAbstractStream<T> stream)
        {
            OnConnected?.Invoke(this);
        }
        #endregion

        #region Disconnect & Dispose region
        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            Disconnect(NetStoppedReason.Manually);
        }

        void Disconnect(NetStoppedReason reason)
        {
            if (!IsConnected)
                return;

            stream.Dispose();

            IsConnected = false;

            OnDisconnected?.Invoke(this, reason);

            stream.OnReceived -= Stream_OnReceived;
            stream.OnStarted -= Stream_OnStarted;
            stream.OnStopped -= Stream_OnStopped;
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        public void Dispose()
        {
            Disconnect();

            RemoteHost = null;
            RemotePort = 0;
        }
        #endregion

        #region Send region
        /// <summary>
        /// Sends the provided data to the server.
        /// </summary>
        /// <param name="data">The data to be sent to the server.</param>
        public async Task<bool> SendAsync(T data)
        {
            return await stream.SendAsync(data);
        }
        #endregion
    }
}
