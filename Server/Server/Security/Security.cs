﻿using System;
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
                aes.IV = IV; 

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
                return ms.ToArray(); 
            }
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = PrivateKey[..32]; // AES-256
                aes.GenerateIV();

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


        public static bool VerifyPassword(string password, string savedPasswordHash)
        {
            // Декодируем сохранённый хеш
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

            // Извлекаем соль (первые 16 байт)
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Вычисляем хеш введённого пароля
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            // Сравниваем хеши
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }
            return true;
        }

    }
}