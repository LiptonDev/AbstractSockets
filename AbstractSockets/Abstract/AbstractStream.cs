﻿using AbstractSockets.Delegates;
using AbstractSockets.Enums;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
        public event AbstractStreamOnStarted<T> OnStarted;

        /// <summary>
        /// Raised when stream was stopped.
        /// </summary>
        public event AbstractStreamOnStopped<T> OnStopped;

        /// <summary>
        /// Raised when stream received new data.
        /// </summary>
        public event AbstractStreamOnReceived<T> OnReceived;

        /// <summary>
        /// Unique GUID of stream.
        /// </summary>
        public Guid Guid { get; }

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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="networkStream">TCP stream.</param>
        /// <param name="endPoint">Endpoint.</param>
        public AbstractStream(NetworkStream networkStream, EndPoint endPoint)
        {
            stream = networkStream;
            EndPoint = endPoint;
        }

        /// <summary>
        /// Should be implemented for handling raw bytes received from the stream.
        /// </summary>
        /// <param name="data">Raw data.</param>
        protected abstract void ReceivedRaw(byte[] data);

        /// <summary>
        /// Sends strictly typed data to the stream.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <returns></returns>
        public abstract bool Send(T data);

        /// <summary>
        /// Starts the stream.
        /// </summary>
        public void Start()
        {
            if (IsActive)
                return;

            receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.Start();

            OnStarted?.Invoke(this);
        }

        /// <summary>
        /// Stops the stream manually locally.
        /// </summary>
        public void Stop()
        {
            Stop(NetStoppedReason.Manually);
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
            stream.Dispose();

            OnStopped?.Invoke(this, reason);
        }


        /// <summary>
        /// Sends a raw byte array to the stream.
        /// </summary>
        /// <param name="data"></param>
        protected bool SendRaw(byte[] data)
        {
            if (!IsActive || !stream.CanWrite)
                return false;

            try
            {
                var bytes = buffer.GetPayload(data);
                stream.Write(bytes, 0, bytes.Length);

                return true;
            }
            catch
            {
                Stop(NetStoppedReason.Remote);
                return false;
            }
        }

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
                    ReceivedRaw(item);
            }

            Stop(NetStoppedReason.Manually);
        }

        protected void RaiseOnReceived(T data)
        {
            OnReceived?.Invoke(this, data);
        }
    }
}