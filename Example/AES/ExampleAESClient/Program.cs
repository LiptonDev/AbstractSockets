using AbstractSockets.Abstract;
using AbstractSockets.Base.Aes;
using AbstractSockets.Enums;
using System;
using System.Text;

namespace ExampleAESClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new AesClient();

            client.OnConnected += OnConnected;
            client.OnDisconnected += OnDisconnected;
            client.OnReceived += OnReceived;

            client.ConnectAsync("127.0.0.1", 29000).Wait();

            while (true)
            {
                var data = Encoding.UTF8.GetBytes(Console.ReadLine());

                client.SendAsync(data).Wait();
            }
        }

        private static void OnReceived(IAbstractClient<byte[]> abstractClient, byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);
            Console.WriteLine($"Recevied \"{str}\" from server");
        }

        private static void OnDisconnected(IAbstractClient<byte[]> abstractClient, NetStoppedReason reason)
        {
            Console.WriteLine("Disconnected");
        }

        private static void OnConnected(IAbstractClient<byte[]> abstractClient)
        {
            Console.WriteLine("Connected");
        }
    }
}
