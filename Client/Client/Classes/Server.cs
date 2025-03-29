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
using Client.Classes;

namespace Client
{
    public class Server
    {
        MainWindow mainWindow;
        ReadAndWrite Messenger = new ReadAndWrite();

        Socket clientSocket;
        string serverIP = Settings1.Default.ServerURL;
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
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(IPAddress.Parse(serverIP), port);
            }
            catch
            {
                MessageBox.Show("Сервер недоступен");
                return;
            }


            Messenger.SendStrings(clientSocket, login + "\a" + password + "\a");
           MessageBox.Show(Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)));
        }
    }
}
