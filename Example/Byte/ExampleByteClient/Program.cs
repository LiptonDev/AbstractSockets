using AbstractSockets.Abstract;
using AbstractSockets.Base.Byte;
using AbstractSockets.Enums;
using System;
using System.Text;

namespace ExampleByteClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ByteClient();

            client.OnConnected += OnConnected;
            client.OnDisconnected += OnDisconnected;
            client.OnReceived += OnReceived;

            client.ConnectAsync("127.0.0.1", 29000).Wait();

            while (true)
            {
                var data = Encoding.UTF8.GetBytes(Console.ReadLine());

                client.SendAsync(new byte[100_000_000]).Wait();
            }
        }

        private static void OnReceived(IAbstractClient<byte[]> abstractClient, byte[] data)
        {
            Console.WriteLine($"Recevied {data.Length} bytes from server");
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
