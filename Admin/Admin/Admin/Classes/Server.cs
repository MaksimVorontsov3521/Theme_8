﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        TcpClient tcpClient;

        string ip = Resources.Settings1.Default.ServerUrl;
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

                // ..!
                //правильный неправильный пароль?
                // ..!

                byte[] jsonBytes = ReedBytes(stream);
                string json = Encoding.UTF8.GetString(jsonBytes);
                receivedUser = JsonSerializer.Deserialize <List<User>>(json);

                jsonBytes = ReedBytes(stream);
                json = Encoding.UTF8.GetString(jsonBytes);
                receivedRole = JsonSerializer.Deserialize<List<Role>>(json);

                //jsonBytes = ReedBytes(stream);
                //json = Encoding.UTF8.GetString(jsonBytes);
                //receivedLog = JsonSerializer.Deserialize<List<Log>>(json);

                jsonBytes = ReedBytes(stream);
                json = Encoding.UTF8.GetString(jsonBytes);
                receivedLogAction = JsonSerializer.Deserialize<List<LogAction>>(json);

                MainWorkPage mainWorkPage = new MainWorkPage(this);
                mainWindow.WorkPlace.Navigate(mainWorkPage);
            }
        }
        private byte[] ReedBytes(NetworkStream stream)
        {
            //
            byte[] lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);
            int length = BitConverter.ToInt32(lengthBytes, 0);
            //
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            //
            return bytes;
        }

        private void SendBytes(NetworkStream stream,string message)
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
    }
}