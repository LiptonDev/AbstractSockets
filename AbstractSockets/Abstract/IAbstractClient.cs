using AbstractSockets.Delegates;
using System.Threading.Tasks;

namespace AbstractSockets.Abstract
{
    /// <summary>
    /// Abstract client interface.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    public interface IAbstractClient<T>
    {
        /// <summary>
        /// Raised when the connection is established succesfully.
        /// </summary>
        event ClientOnConnected<T> OnConnected;

        /// <summary>
        /// Raised when the connection is closed by the client or server.
        /// </summary>
        event ClientOnDisconnected<T> OnDisconnected;

        /// <summary>
        /// Raised when data has been received.
        /// </summary>
        event ClientOnReceived<T> OnReceived;

        /// <summary>
        /// Gets the remote host name.
        /// </summary>
        string RemoteHost { get; }

        /// <summary>
        /// Gets the remote port number.
        /// </summary>
        int RemotePort { get; }

        /// <summary>
        /// Gets a value indicating whether the client is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to the server on the specified host and port.
        /// </summary>
        /// <param name="host">The remote host name.</param>
        /// <param name="port">The remote port number.</param>
        Task ConnectAsync(string host, int port);

        /// <summary>
        /// Connects to the server, returns a bool indicating whether the attempt failed or succeeded.
        /// </summary>
        /// <param name="host">The remote host name.</param>
        /// <param name="port">The remote port number.</param>
        /// <returns></returns>
        Task<bool> TryConnectAsync(string host, int port);

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends the provided data to the server.
        /// </summary>
        /// <param name="data">The data to be sent to the server.</param>
        Task<bool> SendAsync(T data);
    }
}
