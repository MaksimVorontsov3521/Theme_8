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

namespace Server.AdminFolder
{
    public class Admin
    {

        string ipAddress = "127.0.0.1";
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


            Console.WriteLine("AdminEntered");
            TablesForAdmin tablesForAdmin = new TablesForAdmin(dataBase);

            List<UserTable> users = tablesForAdmin.UserTable();
            Messenger.SendJSON(adminSocket, users);

            List<RoleTable> role = tablesForAdmin.RoleTable();
            Messenger.SendJSON(adminSocket, role);

            //List<Log> log = tablesForAdmin.LogTable();
            //Messenger.SendJSON(stream, log);

            //List<LogAction> logActions = tablesForAdmin.LogActionTable();
            //Messenger.SendJSON(adminSocket, logActions);

            List<Department> department = tablesForAdmin.Department();
            Messenger.SendJSON(adminSocket, department);

            //
            //
            //

            AdminAdd adminAdd = new AdminAdd();
            AdminUpdateSetting adminUpdateSetting = new AdminUpdateSetting();
            bool whileBoll = true;
            while (whileBoll)
            {
                string[] AdminCommand = Encoding.UTF8.GetString(await Messenger.ReedBytes(adminSocket)).Split('\a');
                switch (AdminCommand[0])
                {
                    case "UpdateBaseFolder":
                        adminUpdateSetting.UpdateBaseFolder(AdminCommand[1]);
                        break;
                    case "AddDepartment":
                        adminAdd.AddDepartment(AdminCommand[1]);
                        break;
                    case "UpdateClient":
                        byte[] UpdateUser = await Messenger.ReedBytes(adminSocket);
                        adminAdd.UpdateClient(UpdateUser);
                        Messenger.SendStrings(adminSocket,"Успешно");
                        break;
                    case "AddClient":
                        byte[] AddUser = await Messenger.ReedBytes(adminSocket);
                        adminAdd.AddClient(AddUser);
                        Messenger.SendStrings(adminSocket, "Успешно");
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        whileBoll = false;
                        break;
                }
            }
        }
    }
}
