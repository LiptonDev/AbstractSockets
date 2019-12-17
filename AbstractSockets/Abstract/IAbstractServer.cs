using AbstractSockets.Delegates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;

namespace AbstractSockets.Abstract
{
    /// <summary>
    /// Abstract server interface.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    public interface IAbstractServer<T>
    {
        /// <summary>
        /// Raised when the server is started.
        /// </summary>
        event ServerOnStarted<T> OnStarted;

        /// <summary>
        /// Raised when the server is stopped.
        /// </summary>
        event ServerOnStopped<T> OnStopped;

        /// <summary>
        /// Raised when a new client connects.
        /// </summary>
        event ServerOnClientConnected<T> OnClientConnected;

        /// <summary>
        /// Raised when a client disconnects.
        /// </summary>
        event ServerOnClientDisconnected<T> OnClientDisconnected;

        /// <summary>
        /// Raised when an error occurs.
        /// </summary>
        event ServerOnException<T> OnException;

        /// <summary>
        /// Raised when data is received from a client.
        /// </summary>
        event ServerOnDataReceived<T> OnDataReceived;

        /// <summary>
        /// Gets the listening address.
        /// </summary>
        IPAddress Address { get; }

        /// <summary>
        /// Gets the listening port number.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets a value indicating whether the server is online.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Gets an readonly collection of connected client guids.
        /// </summary>
        IEnumerable<Guid> Clients { get; }

        /// <summary>
        /// Gets a readonly dictionary of client streams.
        /// </summary>
        ReadOnlyDictionary<Guid, IAbstractStream<T>> Streams { get; }

        /// <summary>
        /// Starts the server on the specified port and address.
        /// </summary>
        /// <param name="address">The listening address.</param>
        /// <param name="port">The listening port.</param>
        void Start(IPAddress address, int port);

        /// <summary>
        /// Stops the server manually.
        /// </summary>
        void Stop();

        /// <summary>
        /// Disconnect a client.
        /// </summary>
        /// <param name="guid">Guid.</param>
        void DisconnectClient(Guid guid);

        /// <summary>
        /// Disconnect all clients.
        /// </summary>
        void DisconnectAll();

        /// <summary>
        /// Dispatch data to a single client.
        /// </summary>
        /// <param name="guid">Guid.</param>
        /// <param name="data">Data.</param>
        Task<bool> DispatchToAsync(Guid guid, T data);

        /// <summary>
        /// Dispatch data to a group of clients.
        /// </summary>
        /// <param name="guids">Array of client guids.</param>
        /// <param name="data">Data.</param>
        Task<bool> DispatchToAsync(IEnumerable<Guid> guids, T data);

        /// <summary>
        /// Dispatch data to all clients.
        /// </summary>
        /// <param name="data">Data.</param>
        Task<bool> DispatchAllAsync(T data);

        /// <summary>
        /// Dispatch data to all clients except a single client.
        /// </summary>
        /// <param name="guid">Guid.</param>
        /// <param name="data">Data.</param>
        Task<bool> DispatchAllExceptAsync(Guid guid, T data);
    }
}
