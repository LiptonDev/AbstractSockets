using AbstractSockets.Delegates;
using AbstractSockets.Enums;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AbstractSockets.Abstract
{
    /// <summary>
    /// Abstract class for TCP stream.
    /// </summary>
    public abstract class AbstractStream<T> : IAbstractStream<T>
    {
        /// <summary>
        /// Buffer size.
        /// </summary>
        const int BufferSize = 10 * 1024; //10 KB

        /// <summary>
        /// Raised when stream is started.
        /// </summary>
        public event StreamOnStarted<T> OnStarted;

        /// <summary>
        /// Raised when stream was stopped.
        /// </summary>
        public event StreamOnStopped<T> OnStopped;

        /// <summary>
        /// Raised when stream received new data.
        /// </summary>
        public event StreamOnReceived<T> OnReceived;

        /// <summary>
        /// Unique GUID of stream.
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the stream is active.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the remote endpoint.
        /// </summary>
        public EndPoint EndPoint { get; private set; }

        /// <summary>
        /// Tcp stream.
        /// </summary>
        readonly NetworkStream stream;

        /// <summary>
        /// Thread for receiving.
        /// </summary>
        Thread receiveThread;

        /// <summary>
        /// Helper for tcp buffer.
        /// </summary>
        TCPBuffer buffer;

        protected bool IsServerStream { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="networkStream">TCP stream.</param>
        /// <param name="endPoint">Endpoint.</param>
        public AbstractStream(NetworkStream networkStream, EndPoint endPoint, bool isServerStream)
        {
            stream = networkStream;
            EndPoint = endPoint;
            buffer = new TCPBuffer();

            IsServerStream = isServerStream;
        }

        /// <summary>
        /// Send GUID to client.
        /// </summary>
        private async Task SendGuidToClient()
        {
            Guid = GuidHelper.GetGuid();

            await stream.WriteAsync(Guid.ToByteArray(), 0, 16);
        }

        /// <summary>
        /// Get GUID from server.
        /// </summary>
        private async Task RecvGuidFromServer()
        {
            var guid = new byte[16];
            await stream.ReadAsync(guid, 0, 16);

            Guid = new Guid(guid);
        }

        /// <summary>
        /// Should be implemented for handling raw bytes received from the stream.
        /// </summary>
        /// <param name="data">Raw data.</param>
        protected abstract T ReceivedRaw(byte[] data);

        /// <summary>
        /// Sends strictly typed data to the stream.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <returns></returns>
        public abstract Task<bool> SendAsync(T data);

        /// <summary>
        /// Raised when stream starting.
        /// </summary>
        /// <param name="ns">Network stream.</param>
        protected async virtual Task PreStart(NetworkStream ns)
        {
            if (IsServerStream)
                await SendGuidToClient();
            else await RecvGuidFromServer();
        }

        /// <summary>
        /// Starts the stream.
        /// </summary>
        public async void Start()
        {
            if (IsActive)
                return;

            await PreStart(stream);

            receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.Start();

            IsActive = true;

            OnStarted?.Invoke(this);
        }

        /// <summary>
        /// Stops the stream with a specific reason.
        /// </summary>
        /// <param name="reason">The stop reason.</param>
        private void Stop(NetStoppedReason reason)
        {
            if (!IsActive)
                return;

            IsActive = false;
            stream.Close();

            OnStopped?.Invoke(this, reason);

            if (IsServerStream)
                GuidHelper.RemoveGuid(Guid);
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        public virtual void Dispose()
        {
            Stop(NetStoppedReason.Manually);

            EndPoint = null;
            Guid = Guid.Empty;
            stream.Dispose();
            buffer.Dispose();
        }


        /// <summary>
        /// Sends a raw byte array to the stream.
        /// </summary>
        /// <param name="data">Raw bytes.</param>
        protected async Task<bool> SendRawAsync(byte[] data)
        {
            if (!IsActive || !stream.CanWrite)
                return false;

            try
            {
                var bytes = buffer.GetPayload(data);
                await stream.WriteAsync(bytes, 0, bytes.Length);

                return true;
            }
            catch
            {
                Stop(NetStoppedReason.Remote);
                return false;
            }
        }

        /// <summary>
        /// Receiving data from stream.
        /// </summary>
        private void Receive()
        {
            while (IsActive && stream.CanRead)
            {
                byte[] buffer = new byte[BufferSize];
                int recv = 0;

                try
                {
                    recv = stream.Read(buffer, 0, BufferSize);

                    if (recv == 0) //disconnected
                    {
                        Stop(NetStoppedReason.Remote);
                        return;
                    }
                }
                catch (IOException)
                {
                    Stop(NetStoppedReason.Remote);
                    return;
                }
                catch (Exception ex)
                {
                    Stop(NetStoppedReason.Exception);
                    throw ex;
                }

                if (recv < BufferSize)
                {
                    byte[] temp = new byte[recv];
                    Buffer.BlockCopy(buffer, 0, temp, 0, recv);
                    buffer = temp;
                }

                foreach (var item in this.buffer.BufferProcessing(buffer))
                    OnReceived?.Invoke(this, ReceivedRaw(item));
            }

            Stop(NetStoppedReason.Manually);
        }
    }
}
