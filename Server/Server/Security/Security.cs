using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Server.Security
{
    internal class Security
    {
        private Socket socket;
        private byte[] PublicKey { get; set; }
        private byte[] PrivateKey;
        public Security(Socket Socket)
        {
            socket = Socket;
            using (ECDiffieHellman client = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256))
            {
                PublicKey = client.PublicKey.ExportSubjectPublicKeyInfo();
                socket.Send(PublicKey);
                byte[] clientPublicKey = new byte[1024];
                int receivedBytes = socket.Receive(clientPublicKey);

                if (receivedBytes > 0)
                {
                    using (ECDiffieHellman bobPublic = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256))
                    {
                        bobPublic.ImportSubjectPublicKeyInfo(clientPublicKey, out _);

                        // 5. Алиса вычисляет общий секрет
                        PrivateKey = client.DeriveKeyFromHash(
                            bobPublic.PublicKey,
                            HashAlgorithmName.SHA256,
                            null,
                            null);
                    }
                }
            }
        }

        public byte[] Decrypt(byte[] FullMessage, byte[] IV)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = PrivateKey[..32]; // AES-256
                aes.IV = IV; // В реальном коде IV должен быть случайным

                FullMessage = (DecryptMessage(aes, FullMessage));

                return FullMessage;
            }

        }

        static byte[] DecryptMessage(Aes aes, byte[] cipherText)
        {
            using (var decryptor = aes.CreateDecryptor())
            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(cipherText, 0, cipherText.Length);
                }
                return ms.ToArray(); // Возвращаем сырые байты
            }
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = PrivateKey[..32]; // AES-256
                aes.GenerateIV(); // В реальном коде IV должен быть случайным

                socket.Send(aes.IV);

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plainBytes, 0, plainBytes.Length);
                    }
                    return ms.ToArray();
                }

            }
        }


    }
}