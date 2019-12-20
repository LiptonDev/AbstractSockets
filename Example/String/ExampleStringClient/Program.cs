using AbstractSockets.Abstract;
using AbstractSockets.Base.String;
using AbstractSockets.Enums;
using System;
using System.Text;

namespace ExampleStringClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new StringClient(Encoding.UTF8);

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
}
