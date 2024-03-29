﻿using AbstractSockets.Delegates;
using AbstractSockets.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AbstractSockets.Abstract
{
    /// <summary>
    /// Abstract class for TCP server.
    /// </summary>
    public abstract class AbstractServer<T> : StreamCreator<T>, IAbstractServer<T>
    {
        #region protected vars
        protected List<Guid> clients;
        protected Dictionary<Guid, IAbstractStream<T>> streams;
        protected TcpListener listener;
        #endregion

        #region Events
        /// <summary>
        /// Raised when the server is started.
        /// </summary>
        public event ServerOnStarted<T> OnStarted;

        /// <summary>
        /// Raised when the server is stopped.
        /// </summary>
        public event ServerOnStopped<T> OnStopped;

        /// <summary>
        /// Raised when a new client connects.
        /// </summary>
        public event ServerOnClientConnected<T> OnClientConnected;

        /// <summary>
        /// Raised when a client disconnects.
        /// </summary>
        public event ServerOnClientDisconnected<T> OnClientDisconnected;

        /// <summary>
        /// Raised when an error occurs.
        /// </summary>
        public event ServerOnException<T> OnException;

        /// <summary>
        /// Raised when data is received from a client.
        /// </summary>
        public event ServerOnDataReceived<T> OnDataReceived;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the listening address.
        /// </summary>
        public IPAddress Address { get; private set; }

        /// <summary>
        /// Gets the listening port number.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the server is online.
        /// </summary>
        public bool IsOnline { get; private set; }

        /// <summary>
        /// Gets an readonly collection of connected client guids.
        /// </summary>
        public IEnumerable<Guid> Clients => clients.AsReadOnly();

        /// <summary>
        /// Gets a readonly dictionary of client streams.
        /// </summary>
        public ReadOnlyDictionary<Guid, IAbstractStream<T>> Streams => new ReadOnlyDictionary<Guid, IAbstractStream<T>>(streams);
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbstractServer()
        {
            clients = new List<Guid>();
            streams = new Dictionary<Guid, IAbstractStream<T>>();
        }

        #region Start & AcceptClients region
        /// <summary>
        /// Starts the server on the specified port and address.
        /// </summary>
        /// <param name="address">The listening address.</param>
        /// <param name="port">The listening port.</param>
        public void Start(IPAddress address, int port)
        {
            if (IsOnline)
            {
                throw new Exception("Server already started.");
            }

            Address = address;
            Port = port;
            IsOnline = true;

            listener = new TcpListener(address, port);
            listener.Start();

            OnStarted?.Invoke(this);

            AcceptClients();
        }

        private async void AcceptClients()
        {
            while (IsOnline)
            {
                IAbstractStream<T> stream = null;
                TcpClient client = null;
                NetworkStream ns = null;

                try
                {
                    client = await listener.AcceptTcpClientAsync();
                    ns = client.GetStream();
                }
                catch (SocketException socketEx)
                {
                    OnException?.Invoke(this, socketEx);
                    ns?.Dispose();
                    client?.Dispose();

                    continue;
                }
                catch
                {
                    return;
                }

                stream = CreateStream(ns, client.Client.RemoteEndPoint);
                stream.OnStopped += Stream_OnStopped;
                stream.OnStarted += Stream_OnStarted;
                stream.OnReceived += Stream_OnReceived;

                stream.Start();
            }
        }
        #endregion

        #region Stream events
        private void Stream_OnReceived(IAbstractStream<T> stream, T data)
        {
            OnDataReceived?.Invoke(this, stream, data);
        }

        private void Stream_OnStarted(IAbstractStream<T> stream)
        {
            clients.Add(stream.Guid);
            streams.Add(stream.Guid, stream);

            OnClientConnected?.Invoke(this, stream);
        }

        private void Stream_OnStopped(IAbstractStream<T> stream, NetStoppedReason reason)
        {
            clients.Remove(stream.Guid);
            streams.Remove(stream.Guid);

            stream.OnReceived -= Stream_OnReceived;
            stream.OnStarted -= Stream_OnStarted;
            stream.OnStopped -= Stream_OnStopped;

            OnClientDisconnected?.Invoke(this, stream.Guid, reason);
        }
        #endregion

        #region Stop & Dispose region
        /// <summary>
        /// Stops the server manually.
        /// </summary>
        public void Stop()
        {
            if (!IsOnline)
                return;

            DisconnectAll();
            listener.Stop();

            IsOnline = false;

            OnStopped?.Invoke(this, NetStoppedReason.Manually);
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        public void Dispose()
        {
            Stop();

            listener.Server.Dispose();
            Address = null;
            Port = 0;
            IsOnline = false;
        }
        #endregion

        #region Disconnect region
        /// <summary>
        /// Disconnect a client.
        /// </summary>
        /// <param name="guid">Guid.</param>
        public void DisconnectClient(Guid guid)
        {
            if (!streams.ContainsKey(guid))
                return;

            streams[guid].Dispose();
        }

        /// <summary>
        /// Disconnect all clients.
        /// </summary>
        public void DisconnectAll()
        {
            while (clients.Count > 0)
            {
                streams[clients[0]].Dispose();
            }
        }
        #endregion

        #region Dispatch region
        /// <summary>
        /// Dispatch data to a single client.
        /// </summary>
        /// <param name="guid">Guid.</param>
        /// <param name="data">Data.</param>
        public async Task<bool> DispatchToAsync(Guid guid, T data)
        {
            return streams.ContainsKey(guid) && await streams[guid].SendAsync(data);
        }

        /// <summary>
        /// Dispatch data to a group of clients.
        /// </summary>
        /// <param name="guids">Array of client guids.</param>
        /// <param name="data">Data.</param>
        public async Task<bool> DispatchToAsync(IEnumerable<Guid> guids, T data)
        {
            return await DispatchToClients(guids, data);
        }

        /// <summary>
        /// Dispatch data to all clients.
        /// </summary>
        /// <param name="data">Data.</param>
        public async Task<bool> DispatchAllAsync(T data)
        {
            return await DispatchToClients(clients, data);
        }

        /// <summary>
        /// Dispatch data to all clients except a single client.
        /// </summary>
        /// <param name="guid">Guid.</param>
        /// <param name="data">Data.</param>
        public async Task<bool> DispatchAllExceptAsync(Guid guid, T data)
        {
            return await DispatchToClients(clients.Where(x => x != guid), data);
        }

        async Task<bool> DispatchToClients(IEnumerable<Guid> guids, T data)
        {
            var tasks = guids.AsParallel().Select(x => DispatchToAsync(x, data));
            var res = await Task.WhenAll(tasks);

            return res.All(x => x);
        }
        #endregion
    }
}
