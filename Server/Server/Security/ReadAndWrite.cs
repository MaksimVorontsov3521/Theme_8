using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Server.Security
{
    internal class ReadAndWrite
    {
        Security security;
        internal ReadAndWrite(Security Security)
        {
            security = Security;
        }

        internal async void SendJSON(Socket socket, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            jsonBytes = security.Encrypt(jsonBytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные       
            await socket.SendAsync(jsonBytes);
        }

        internal void SendStrings(Socket socket, string stringData)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(stringData);
            Bytes = security.Encrypt(Bytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(Bytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные   
            socket.SendAsync(Bytes);
        }

        internal void SendBytes(Socket socket, byte[] Bytes)
        {
            Bytes = security.Encrypt(Bytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(Bytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные    
            socket.SendAsync(Bytes);
        }

        internal async Task<byte[]> ReedBytes(Socket socket)
        {
            try
            {
                byte[] IV = new byte[16];
                socket.Receive(IV);

                // Читаем длину сообщения (первые 4 байта)
                byte[] lengthBuffer = new byte[4];
                socket.Receive(lengthBuffer);
                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);


                // Читаем само сообщение
                byte[] messageBuffer = new byte[messageLength];
                int bytesRead = 0;

                while (bytesRead < messageLength)
                {
                    bytesRead += await socket.ReceiveAsync(messageBuffer);
                    messageBuffer = security.Decrypt(messageBuffer, IV);
                }
                return messageBuffer;
            }
            catch
            {
                byte[] Error = new byte[4];
                return Error;
            }
        }
    }
}
