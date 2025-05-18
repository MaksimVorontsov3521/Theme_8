using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Admin.Classes
{
    internal class ReadAndWrite
    {
        Security security;
        public ReadAndWrite(Socket socket)
        {
            Security Security = new Security(socket);
            security = Security;
        }
        public async Task SendJSON(Socket socket, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            jsonBytes = await security.Encrypt(jsonBytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные       
            await socket.SendAsync(jsonBytes);
        }

        public async Task SendStrings(Socket socket, string stringData)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(stringData);
            Bytes = await security.Encrypt(Bytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(Bytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные   
            socket.SendAsync(Bytes);
        }

        public async Task SendBytes(Socket socket, byte[] Bytes)
        {
            Bytes = await security.Encrypt(Bytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(Bytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные    
            socket.SendAsync(Bytes);
        }

        public async Task<byte[]> ReedBytes(Socket socket)
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
                using var ms = new MemoryStream();

                while (bytesRead < messageLength)
                {
                    bytesRead += await socket.ReceiveAsync(messageBuffer);
                    messageBuffer = await security.Decrypt(messageBuffer, IV);
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