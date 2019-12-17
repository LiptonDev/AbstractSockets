using AbstractSockets.Abstract;
using AbstractSockets.Base;
using AbstractSockets.Enums;
using System;
using System.Net;

namespace ExampleByteServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ByteServer();

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

        private static void Server_OnClientConnected(IAbstractServer<byte[]> abstractServer, Guid guid)
        {
            Console.WriteLine($"{guid} - Connected!");
        }

        private static void Server_OnException(IAbstractServer<byte[]> abstractServer, Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
        }

        private static async void Server_OnDataReceived(IAbstractServer<byte[]> abstractServer, Guid guid, byte[] data)
        {
            Console.WriteLine($"Recevied {data.Length} bytes from {guid}");

            await abstractServer.Streams[guid].SendAsync(data);
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
