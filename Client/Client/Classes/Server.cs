using Client.Pages;
using Client.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Text.Json;

namespace Client
{
    public class Server
    {
        MainWindow mainWindow;
        TcpClient tcpClient;

        string ip = Settings1.Default.ServerURL;
        int port = Settings1.Default.ServerPort;

        public Server(MainWindow mainWindow)
        {
            // Связка окна и сервера
            this.mainWindow = mainWindow;
            Server server = this;
            Entrance entrance = new Entrance(server);
            mainWindow.WorkPlace.Navigate(entrance);
        }

        public void Connection(string login, string password)
        {

            try
            {
                tcpClient = new TcpClient(ip, port);
            }
            catch
            {
                MessageBox.Show("Сервер недоступен");
                return;
            }

            using (NetworkStream stream = tcpClient.GetStream())
            {
                SendBytes(stream,login + "\a" + password + "\a");
                byte[] buffer = ReedBytes(stream);

            }
        }

        public void SendBytes(NetworkStream stream,string message)
        {
            //
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            //
            byte[] length = BitConverter.GetBytes(bytes.Length);
            //
            stream.Write(length, 0, 4);
            //
            stream.Write(bytes, 0, bytes.Length);
        }

        private byte[] ReedBytes(NetworkStream stream)
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
