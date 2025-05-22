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

            GetTables(Messenger, clientSocket);

            MainWorkPage mainWorkPage = new MainWorkPage(this);
            mainWindow.WorkPlace.Navigate(mainWorkPage);
        }

        internal async void GetTables(ReadAndWrite messenger, Socket clientSocket)
        {
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
        }


        internal async void CreateNewRole(Role role)
        {
            await Messenger.SendStrings(clientSocket, "CreateNewRole\a");
            await Messenger.SendJSON(clientSocket,role);
            byte[] bytes = await Messenger.ReedBytes(clientSocket);
            StyleClass.TransactionResult(Encoding.UTF8.GetString(bytes), mainWindow);
            GetTables(Messenger, clientSocket);
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
            GetTables(Messenger, clientSocket);
        }

        internal async void AddDepartment(string DepartmentName)
        {
            await Messenger.SendStrings(clientSocket, "AddDepartment\a" + DepartmentName);          
            GetTables(Messenger, clientSocket);
            byte[] bytes = await Messenger.ReedBytes(clientSocket);
            StyleClass.TransactionResult(Encoding.UTF8.GetString(bytes), mainWindow);
        }

        internal async void UpdateConnectionStrings(string connectionString)
        {
            await Messenger.SendStrings(clientSocket, "UpdateConnectionStrings\a" + connectionString);
            byte[] bytes = await Messenger.ReedBytes(clientSocket);
            StyleClass.TransactionResult(Encoding.UTF8.GetString(bytes), mainWindow);
        }

        internal async void UpdateClient(User user)
        {
            await Messenger.SendStrings(clientSocket, "UpdateClient\a");
            await Messenger.SendJSON(clientSocket, user);
            byte[] bytes =  await Messenger.ReedBytes(clientSocket);
            StyleClass.TransactionResult(Encoding.UTF8.GetString(bytes), mainWindow);
            GetTables(Messenger, clientSocket);
            
        }

        internal async void ChangeServerSettingsTwo(int UserPort, int AdminPort, int CanCreateNewProject,int CanEditClient, string ServerUrl)
        {
            int[] ints = new int[4];
            ints[0] = UserPort;
            ints[1] = AdminPort;
            ints[2] = CanCreateNewProject;
            ints[3] = CanEditClient;
            await Messenger.SendStrings(clientSocket, "ChangeServerSettingsTwo\a");
            await Messenger.SendJSON(clientSocket, ints);
            await Messenger.SendStrings(clientSocket, ServerUrl);

            byte[] bytes = await Messenger.ReedBytes(clientSocket);
            StyleClass.TransactionResult(Encoding.UTF8.GetString(bytes), mainWindow);
            GetTables(Messenger, clientSocket);

        }

        internal async void AddClient(User user)
        {
            await Messenger.SendStrings(clientSocket, "AddClient\a");
            await Messenger.SendJSON(clientSocket, user);
            byte[] bytes = await Messenger.ReedBytes(clientSocket);
            StyleClass.TransactionResult(Encoding.UTF8.GetString(bytes), mainWindow);
            GetTables(Messenger, clientSocket);

        }
        internal void PrintGrids(MainWorkPage mainWorkPage)
        {
            mainWorkPage.PrintGrids();
        }

    }
}