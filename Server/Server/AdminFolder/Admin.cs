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
            using (TcpClient client = adminServer.AcceptTcpClient())
            {
                using (NetworkStream stream = client.GetStream())
                {
                    string message = ReedString(stream);
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
                        HandelAdmin();
                        Console.WriteLine("Неправильный ввод от компьютера ");
                    }


                    if (a == true)
                    {
                        Console.WriteLine("Entered");
                        TablesForAdmin tablesForAdmin = new TablesForAdmin(dataBase);
                        List<User> users = tablesForAdmin.UserTable();


                        Send(stream, users);
                    }
                    else
                    {
                        Console.WriteLine("Неверный пароль");
                        HandelAdmin();
                    }
                }
            }
           
        }

        private void Send(NetworkStream stream,object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            // Сначала отправляем длину данных
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            stream.Write(lengthBytes, 0, 4);

            // Затем отправляем сами данные
            stream.Write(jsonBytes, 0, jsonBytes.Length);

            Console.WriteLine("Данные отправлены");
        }

        private string ReedString(NetworkStream stream)
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

            string message = Encoding.UTF8.GetString(messageBuffer);
            Console.WriteLine($"Получено: {message}");
            return message;

        }

        private static byte[] ObjectToByteArray(object obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}
