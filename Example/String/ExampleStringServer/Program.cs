using AbstractSockets.Abstract;
using AbstractSockets.Base.String;
using AbstractSockets.Enums;
using System;
using System.Net;
using System.Text;

namespace ExampleStringServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new StringServer(Encoding.UTF8);

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

        private static void Server_OnClientDisconnected(IAbstractServer<string> abstractServer, Guid guid, NetStoppedReason reason)
        {
            Console.WriteLine($"{guid} - Disconnected!");
        }

        private static void Server_OnClientConnected(IAbstractServer<string> abstractServer, IAbstractStream<string> stream)
        {
            Console.WriteLine($"{stream.Guid} - Connected!");
        }

        private static void Server_OnException(IAbstractServer<string> abstractServer, Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
        }

        private static async void Server_OnDataReceived(IAbstractServer<string> abstractServer, IAbstractStream<string> stream, string data)
        {
            Console.WriteLine($"Recevied \"{data}\" from {stream.Guid}");

            await abstractServer.DispatchAllAsync(data);
        }

        private static void Server_OnStopped(IAbstractServer<string> abstractServer, NetStoppedReason reason)
        {
            Console.WriteLine($"Server stopped");
        }

        private static void Server_OnStarted(IAbstractServer<string> abstractServer)
        {
            Console.WriteLine("Server started");
        }
    }
}
