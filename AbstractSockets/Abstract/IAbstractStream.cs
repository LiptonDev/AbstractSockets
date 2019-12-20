using AbstractSockets.Delegates;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AbstractSockets.Abstract
{
    /// <summary>
    /// Abstract stream interface.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    public interface IAbstractStream<T> : IDisposable
    {
        /// <summary>
        /// Raised when stream is started.
        /// </summary>
        event StreamOnStarted<T> OnStarted;

        /// <summary>
        /// Raised when stream was stopped.
        /// </summary>
        event StreamOnStopped<T> OnStopped;

        /// <summary>
        /// Raised when stream received new data.
        /// </summary>
        event StreamOnReceived<T> OnReceived;

        /// <summary>
        /// Unique GUID of stream.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Gets a value indicating whether the stream is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets the remote endpoint.
        /// </summary>
        EndPoint EndPoint { get; }

        /// <summary>
        /// Sends strictly typed data to the stream.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <returns></returns>
        Task<bool> SendAsync(T data);

        /// <summary>
        /// Starts the stream.
        /// </summary>
        void Start();
    }
}
