using AbstractSockets.Abstract;
using AbstractSockets.Enums;
using ExampleStringStream;
using System;
using System.Net;
using System.Net.Sockets;

namespace ExampleStringClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new StringClient();

            client.OnConnected += OnConnected;
            client.OnDisconnected += OnDisconnected;
            client.OnReceived += OnReceived;

            client.ConnectAsync("127.0.0.1", 29000).Wait();

            while (true)
            {
                client.SendAsync(Console.ReadLine()).Wait();
            }
        }

        private static void OnReceived(IAbstractClient<string> abstractClient, string data)
        {
            Console.WriteLine($"Recevied \"{data}\" from server");
        }

        private static void OnDisconnected(IAbstractClient<string> abstractClient, NetStoppedReason reason)
        {
            Console.WriteLine("Disconnected");
        }

        private static void OnConnected(IAbstractClient<string> abstractClient)
        {
            Console.WriteLine("Connected");
        }
    }

    class StringClient : AbstractClient<string>
    {
        protected override IAbstractStream<string> CreateStream(NetworkStream ns, EndPoint ep)
        {
            return new StringStream(ns, ep, false);
        }
    }
}
