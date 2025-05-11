using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Client.Classes
{
    internal class ReadAndWrite
    {

        Security security;
        internal ReadAndWrite(Socket socket)
        {
            Security Security = new Security(socket);
            security = Security;
        }

        internal void SendJSON(Socket socket, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            jsonBytes = security.Encrypt(jsonBytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные       
            socket.Send(jsonBytes);      
        }

        internal void SendStrings(Socket socket, string stringData)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(stringData);
            Bytes = security.Encrypt(Bytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(Bytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные   
            socket.Send(Bytes);
        }

        internal void SendBytes(Socket socket, byte[] Bytes)
        {
            Bytes = security.Encrypt(Bytes);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(Bytes.Length);
            socket.Send(lengthBytes);

            // Затем отправляем сами данные    
            socket.Send(Bytes);
        }

        internal byte[] ReedBytes(Socket socket)
        {
            //try
            //{

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
                    bytesRead += socket.Receive(messageBuffer);
                    messageBuffer = security.Decrypt(messageBuffer, IV);
                }
                Console.WriteLine(Encoding.UTF8.GetString(messageBuffer));
                return messageBuffer;
            //}
            //catch
            //{
            //    byte[] Error = new byte[4];
            //    return Error;
            //}
        }
    }
}
