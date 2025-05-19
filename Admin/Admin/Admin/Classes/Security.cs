using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Classes
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
                byte[] serverPublicKey = new byte[1024];
                int receivedBytes = socket.Receive(serverPublicKey);
                PublicKey = client.PublicKey.ExportSubjectPublicKeyInfo();
                socket.Send(PublicKey);

                if (receivedBytes > 0)
                {
                    using (ECDiffieHellman bobPublic = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256))
                    {
                        bobPublic.ImportSubjectPublicKeyInfo(serverPublicKey, out _);

                        // 5. Алиса вычисляет общий секрет
                        PrivateKey = client.DeriveKeyFromHash(
                            bobPublic.PublicKey,
                            HashAlgorithmName.SHA256, null, null);
                    }
                }
            }
        }

        public async Task<byte[]> Encrypt(byte[] plainBytes)
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

        public async Task<byte[]> Decrypt(byte[] FullMessage, byte[] IV)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = PrivateKey[..32]; // AES-256
                aes.IV = IV; // В реальном коде IV должен быть случайным

                FullMessage = (await DecryptMessage(aes, FullMessage));

                return FullMessage;
            }

        }


        static async Task<byte[]> DecryptMessage(Aes aes, byte[] cipherText)
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

        public static string HashPassword(string password)
        {
            // Генерируем случайную соль
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Настройки PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20); // 20 байт хеша

            // Объединяем соль и хеш
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Конвертируем в Base64 для хранения
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}