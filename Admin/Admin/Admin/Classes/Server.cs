using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Admin.Classes;
using Admin.Pages;
using Admin.Resources;

namespace Admin.Classes
{
    public class Server
    {
        MainWindow mainWindow;
        ReadAndWrite Messenger = new ReadAndWrite();

        Socket clientSocket;
        string serverIP = Resources.Settings1.Default.ServerUrl;
        int port = Resources.Settings1.Default.AdminPort;

        internal List<User> receivedUser;
        internal List<Role> receivedRole;
        internal List<Log> receivedLog;
        internal List<LogAction> receivedLogAction;


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

            if (Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)).Contains("Right"))
            { }
            else { MessageBox.Show("Неверный логин или пароль"); return; }


            byte[] jsonBytes = Messenger.ReedBytes(clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            receivedUser = JsonSerializer.Deserialize<List<User>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedRole = JsonSerializer.Deserialize<List<Role>>(json);

            //jsonBytes = ReedBytes(clientSocket);
            //json = Encoding.UTF8.GetString(jsonBytes);
            //receivedLog = JsonSerializer.Deserialize<List<Log>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedLogAction = JsonSerializer.Deserialize<List<LogAction>>(json);

            MainWorkPage mainWorkPage = new MainWorkPage(this);
            mainWindow.WorkPlace.Navigate(mainWorkPage);
        }

        public void UpdateBaseFolder(string NewPath)
        {
            Messenger.SendStrings(clientSocket, "UpdateBaseFolder\a"+ NewPath);
        }
    }
}