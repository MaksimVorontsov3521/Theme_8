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
        private int port = Settings.Settings1.Default.AdminPort;
        private TcpListener adminServer;
        async public void Connection()
        {       
            adminServer = new TcpListener(port);
            adminServer.Start();
            Console.WriteLine("Порт для админа запущен");
            HandelAdmin();

        }

        private void HandelAdmin()
        {
            ReadAndWrite Messenger = new ReadAndWrite();

            using (TcpClient client = adminServer.AcceptTcpClient())
            {
                using (NetworkStream stream = client.GetStream())
                {
                    string message = Encoding.UTF8.GetString(Messenger.ReedBytes(stream));
                    // Проверка пароля
                    DataBase dataBase = new DataBase();
                    LoginPassword loginPassword = new LoginPassword(dataBase);
                    bool a = false;
                    //try
                    //{
                        string[] clientLoginPassword = message.Split('\a');
                        a = loginPassword.LoginAdmin(clientLoginPassword[0], clientLoginPassword[1]);
                    //}
                    //catch
                    //{
                    //    HandelAdmin();
                    //    Console.WriteLine("Неправильный ввод от компьютера ");
                    //}


                    if (a == true)
                    {                       
                        Console.WriteLine("Entered");
                        TablesForAdmin tablesForAdmin = new TablesForAdmin(dataBase);

                        List<UserTable> users = tablesForAdmin.UserTable();
                        Messenger.SendBytes(stream, users);

                        List<RoleTable> role = tablesForAdmin.RoleTable();
                        Messenger.SendBytes(stream, role);

                        //List<Log> log = tablesForAdmin.LogTable();
                        //SendBytes(stream, log);

                        List<LogAction> logActions = tablesForAdmin.LogActionTable();
                        Messenger.SendBytes(stream, logActions);
                    }
                    else
                    {
                        Console.WriteLine("Неверный пароль");
                        HandelAdmin();
                        return;
                    }


                    //
                    //
                    //


                    AdminUpdateSetting adminUpdateSetting = new AdminUpdateSetting();
                    bool whileBoll = true;
                    while (whileBoll)
                    {
                        string[] AdminCommand = message.Split('\a');
                        switch (AdminCommand[0])
                        {
                            case "UpdateBaseFolder":
                                adminUpdateSetting.UpdateBaseFolder(AdminCommand[1]);
                                break;
                             default:
                                Console.WriteLine("Неизвестная команда");
                                whileBoll = false;
                                break;
                        }
                    }                   
                }
            }
            return;
        }
    }
}
