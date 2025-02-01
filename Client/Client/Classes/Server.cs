using Client.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection.PortableExecutable;

namespace Client
{
    public class Server
    {
        MainWindow mainWindow;
        Socket soket; 
        string ip = "127.0.0.1";
        int port = 8080;
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
            SendBytes(login+"\a"+password);
        }

        public void SendBytes(string message)
        {
            // Преобразуем строку в массив байтов
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            // Отправляем массив байтов другой программе
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(ip, port);
                socket.Send(bytes);
                while (true)
                {
                    Thread.Sleep(1000);
                    socket.Send(bytes);
                }
            }

        }
    }
}
