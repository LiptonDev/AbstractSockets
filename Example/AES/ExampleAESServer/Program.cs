using AbstractSockets.Abstract;
using AbstractSockets.Base.Aes;
using AbstractSockets.Enums;
using System;
using System.Net;
using System.Text;

namespace ExampleAESServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new AesServer();

            server.OnStarted += Server_OnStarted;
            server.OnStopped += Server_OnStopped;
            server.OnDataReceived += Server_OnDataReceived;
            server.OnException += Server_OnException;
            server.OnClientConnected += Server_OnClientConnected;
            server.OnClientDisconnected += Server_OnClientDisconnected;

            server.Start(IPAddress.Any, 29000);

            Console.ReadLine();

            server.Stop();

            Console.ReadLine();
        }

        private static void Server_OnClientDisconnected(IAbstractServer<byte[]> abstractServer, Guid guid, NetStoppedReason reason)
        {
            Console.WriteLine($"{guid} - Disconnected!");
        }

        private static void Server_OnClientConnected(IAbstractServer<byte[]> abstractServer, IAbstractStream<byte[]> stream)
        {
            Console.WriteLine($"{stream.Guid} - Connected!");
        }

        private static void Server_OnException(IAbstractServer<byte[]> abstractServer, Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
        }

        private static async void Server_OnDataReceived(IAbstractServer<byte[]> abstractServer, IAbstractStream<byte[]> stream, byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);
            Console.WriteLine($"Recevied \"{str}\" from {stream.Guid}");

            await abstractServer.DispatchAllAsync(data);
        }

        private static void Server_OnStopped(IAbstractServer<byte[]> abstractServer, NetStoppedReason reason)
        {
            Console.WriteLine($"Server stopped");
        }

        private static void Server_OnStarted(IAbstractServer<byte[]> abstractServer)
        {
            Console.WriteLine("Server started");
        }
    }
}
