using AbstractSockets.Abstract;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Crypto = System.Security.Cryptography;

namespace AbstractSockets.Base.Aes
{
    public class AesStream : AbstractStream<byte[]>
    {
        Crypto.Aes aes;
        byte[] key = null;
        byte[] iv = null;

        public AesStream(NetworkStream ns, EndPoint ep, bool isServerStream) : base(ns, ep, isServerStream)
        {
            iv = Guid.ToByteArray();
            aes = Crypto.Aes.Create();
        }

        protected async override Task PreStart(NetworkStream ns)
        {
            await base.PreStart(ns); //ALWAYS!!!

            var guid = Guid.NewGuid();
            var guidArray = guid.ToByteArray();

            if (IsServerStream)
            {
                await ns.WriteAsync(guidArray, 0, 16);
            }
            else
            {
                var buffer = new byte[16];
                await ns.ReadAsync(buffer, 0, 16);
                guidArray = buffer;
            }

            key = Crypto.MD5.Create().ComputeHash(guidArray);
        }

        public async override Task<bool> SendAsync(byte[] data)
        {
            using (var enc = aes.CreateEncryptor(key, iv))
            using (MemoryStream ms = new MemoryStream())
            using (Crypto.CryptoStream cs = new Crypto.CryptoStream(ms, enc, Crypto.CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return await SendRawAsync(ms.ToArray());
            }
        }

        protected override byte[] ReceivedRaw(byte[] data)
        {
            using (var dec = aes.CreateDecryptor(key, iv))
            using (MemoryStream ms = new MemoryStream())
            using (Crypto.CryptoStream cs = new Crypto.CryptoStream(ms, dec, Crypto.CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return ms.ToArray();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            key = null;
            iv = null;
            aes.Dispose();
            aes = null;
        }
    }
}
