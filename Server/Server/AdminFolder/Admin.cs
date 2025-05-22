using Server.DataBaseFolder.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.DataBaseFolder;
using Server.DataBaseFolder.Querys;
using Server.DataBaseFolder.Entitys;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Data.SqlTypes;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Server.Security;
using System.Dynamic;

namespace Server.AdminFolder
{
    public class Admin
    {

        string ipAddress = Settings.Settings1.Default.ServerUrl;
        private int port = Settings.Settings1.Default.AdminPort;

        async public void Connection()
        {
            //AdminUpdateSetting setting = new AdminUpdateSetting();
            //setting.UpdateBaseFolder("D:\\Desktop\\Root");

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            serverSocket.Listen(10); // Backlog
            Console.WriteLine("Порт для админа запущен");
            while (true)
            {               
                Socket adminSocket = serverSocket.Accept();
                HandelAdmin(adminSocket);
            }

        }

        private async void HandelAdmin(Socket adminSocket)
        {
            Security.Security security = new Security.Security(adminSocket);
            ReadAndWrite Messenger = new ReadAndWrite(security);

            string message = Encoding.UTF8.GetString(await Messenger.ReedBytes(adminSocket));
            // Проверка пароля
            DataBase dataBase = new DataBase();
            LoginPassword loginPassword = new LoginPassword(dataBase);

            bool a = false;
            try
            {
                string[] clientLoginPassword = message.Split('\a');
                a = loginPassword.LoginAdmin(clientLoginPassword[0], clientLoginPassword[1]);
            }
            catch
            {
                Console.WriteLine("Неправильный ввод от компьютера ");
                return;
            }

            if (a == true)
            {
                Messenger.SendStrings(adminSocket, "Right");
            }
            else
            {
                Messenger.SendStrings(adminSocket, "Wrong");
                return;
            }

            SendTables(dataBase, Messenger, adminSocket);

            //
            //
            //

            AdminAdd adminAdd = new AdminAdd();
            AdminUpdateSetting adminUpdateSetting = new AdminUpdateSetting();
            bool whileBoll = true;
            string result;
            while (whileBoll)
            {
                string[] AdminCommand = Encoding.UTF8.GetString(await Messenger.ReedBytes(adminSocket)).Split('\a');
                switch (AdminCommand[0])
                {
                    case "UpdateBaseFolder":
                        adminUpdateSetting.UpdateBaseFolder(AdminCommand[1]);
                        SendTables(dataBase, Messenger, adminSocket);
                        break;
                    case "AddDepartment":
                        result = adminAdd.AddDepartment(AdminCommand[1]);
                        SendTables(dataBase, Messenger, adminSocket);
                        Messenger.SendStrings(adminSocket, result);
                        break;
                    case "UpdateClient":
                        byte[] UpdateUser = await Messenger.ReedBytes(adminSocket);
                        adminAdd.UpdateClient(UpdateUser);
                        Messenger.SendStrings(adminSocket,"Успешно");
                        SendTables(dataBase, Messenger, adminSocket);
                        break;
                    case "AddClient":
                        byte[] AddUser = await Messenger.ReedBytes(adminSocket);
                        adminAdd.AddClient(AddUser);
                        Messenger.SendStrings(adminSocket, "Успешно");
                        SendTables(dataBase, Messenger, adminSocket);
                        break;
                    case "CreateNewRole":
                        byte[] newRole = await Messenger.ReedBytes(adminSocket);
                        result = adminAdd.AddRole(newRole);
                        Messenger.SendStrings(adminSocket, "Успешно");
                        SendTables(dataBase, Messenger, adminSocket);
                        break;
                    default:
                        whileBoll = false;
                        adminSocket.Close();
                        break;
                }
            }
        }

        private void SendTables(DataBase dataBase, ReadAndWrite Messenger, Socket adminSocket)
        {
            TablesForAdmin tablesForAdmin = new TablesForAdmin(dataBase);

            List<UserTable> users = tablesForAdmin.UserTable();
            Messenger.SendJSON(adminSocket, users);

            List<RoleTable> role = tablesForAdmin.RoleTable();
            Messenger.SendJSON(adminSocket, role);

            List<Department> department = tablesForAdmin.Department();
            Messenger.SendJSON(adminSocket, department);

            List<object> settingsList = new List<object>();

            settingsList.Add(Settings.Settings1.Default.AdminPort);
            settingsList.Add(Settings.Settings1.Default.UserPort);
            settingsList.Add(Settings.Settings1.Default.ServerUrl);
            settingsList.Add(Settings.Settings1.Default.BaseFolder);

            settingsList.Add(Settings.Settings2.Default.CanCreateNewProject);
            settingsList.Add(Settings.Settings2.Default.CanEditClient);

            Messenger.SendJSON(adminSocket, settingsList);
        }
    }
}
