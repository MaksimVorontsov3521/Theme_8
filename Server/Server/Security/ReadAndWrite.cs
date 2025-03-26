using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Security
{
    internal class ReadAndWrite
    {
        internal void SendBytes(NetworkStream stream, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            stream.Write(lengthBytes, 0, 4);

            // Затем отправляем сами данные
            stream.Write(jsonBytes, 0, jsonBytes.Length);
        }

        internal byte[] ReedBytes(NetworkStream stream)
        {
            // Читаем длину сообщения (первые 4 байта)
            byte[] lengthBuffer = new byte[4];
            stream.Read(lengthBuffer, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // Читаем само сообщение
            byte[] messageBuffer = new byte[messageLength];
            int bytesRead = 0;
            while (bytesRead < messageLength)
            {
                bytesRead += stream.Read(messageBuffer, bytesRead, messageLength - bytesRead);
            }

            return messageBuffer;
        }

    }
}
