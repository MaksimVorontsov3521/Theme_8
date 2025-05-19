using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
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
        ReadAndWrite Messenger;

        Socket clientSocket;
        string serverIP = Resources.Settings1.Default.ServerUrl;
        int port = Resources.Settings1.Default.AdminPort;

        internal List<User> receivedUser;
        internal List<Role> receivedRole;
        //internal List<LogAction> receivedLogAction;
        //internal List<Log> receivedLog;
        internal List<Department> receivedDepartment;


        public Server(MainWindow mainWindow)
        {
            // Связка окна и сервера
            this.mainWindow = mainWindow;
            Server server = this;
            Entrance entrance = new Entrance(server);
            mainWindow.WorkPlace.Navigate(entrance);
        }


        public async void Connection(string login, string password)
        {
            
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.ConnectAsync(IPAddress.Parse(serverIP), port);
                ReadAndWrite messenger = new ReadAndWrite(clientSocket);
                Messenger = messenger;
            }
            catch
            { 
                MessageBox.Show("Сервер недоступен");
                return;
            }

            await Messenger.SendStrings(clientSocket, login + "\a" + password + "\a");

            byte[] get = await Messenger.ReedBytes(clientSocket);
            
            if (Encoding.UTF8.GetString(get).Contains("Right"))
            { }
            else { MessageBox.Show("Неверный логин или пароль"); return; }


            byte[] jsonBytes = await Messenger.ReedBytes(clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            receivedUser = JsonSerializer.Deserialize<List<User>>(json);

            jsonBytes = await Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedRole = JsonSerializer.Deserialize<List<Role>>(json);

            jsonBytes = await Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedDepartment = JsonSerializer.Deserialize<List<Department>>(json);
            
            jsonBytes = await Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            List<JsonElement> settingsList = JsonSerializer.Deserialize<List<JsonElement>>(json);

            GetSettings(settingsList);
            

            MainWorkPage mainWorkPage = new MainWorkPage(this);
            mainWindow.WorkPlace.Navigate(mainWorkPage);
        }

        internal void GetSettings(List<JsonElement> settingsList)
        {
            Settings1.Default.AdminPort = settingsList[0].GetInt32();
            Settings1.Default.UserPort = settingsList[1].GetInt32();
            Settings1.Default.ServerUrl = settingsList[2].GetString();
            Settings1.Default.BaseFolder = settingsList[3].GetString();
            Settings1.Default.CanCreateNewProject = settingsList[4].GetInt32();
            Settings1.Default.CanEditClient = settingsList[5].GetInt32();
            Settings1.Default.Save();
        }
        internal async Task UpdateBaseFolder(string NewPath)
        {
           await Messenger.SendStrings(clientSocket, "UpdateBaseFolder\a"+ NewPath);
        }

        internal async void AddDepartment(string DepartmentName)
        {
           await Messenger.SendStrings(clientSocket, "AddDepartment\a" + DepartmentName);
        }

        internal async void UpdateClient(User user)
        {
            await Messenger.SendStrings(clientSocket, "UpdateClient\a");
            await Messenger.SendJSON(clientSocket, user);
            byte[] bytes =  await Messenger.ReedBytes(clientSocket);
            StyleClass.TransactionResult(Encoding.UTF8.GetString(bytes), mainWindow);
        }

        internal async void AddClient(User user)
        {
            await Messenger.SendStrings(clientSocket, "AddClient\a");
            await Messenger.SendJSON(clientSocket, user);
            byte[] bytes = await Messenger.ReedBytes(clientSocket);
            StyleClass.TransactionResult(Encoding.UTF8.GetString(bytes), mainWindow);
        }
    }
}