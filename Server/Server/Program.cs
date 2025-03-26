using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Server;
using Server.AdminFolder;
using Server.DataBaseFolder;
using Server.DataBaseFolder.DbContexts;
using Server.DataBaseFolder.Entitys;
using Server.DataBaseFolder.Querys;
using Server.Settings;
using System.Net.Sockets;
using System.Resources;
using System.Text;

public class Program
{
    int port = Server.Settings.Settings1.Default.UserPort;
    TcpListener server;
    static void Main(string[] args)
    {
        Console.WriteLine("qwe");
        Program program = new Program();
        program.Setup();
    }
    private void Setup()
    {
        AdminConnection();
        server = new TcpListener(port);
        server.Start();
        Console.WriteLine("Порт для пользователей запущен");
        Connection();
    }
    private void AdminConnection()
    {
        Admin admin = new Admin();
        Thread adminThread = new Thread(() => admin.Connection());
        adminThread.Start();
    }

    private void Connection()
    {
        Thread thread = new Thread(() => HandelClient());      
        thread.Start();
    }

    private List<byte> ReedBytes(TcpClient client)
    {
        List<byte> allBytes = new List<byte>();
        // Создаем поток для чтения
        using (var stream = client.GetStream())
        {
            // Читаем массив байтов из сокета
            byte[] bytes = new byte[1024];
            int bytesRead;
            while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) > 0)
            {
                allBytes.AddRange(bytes);
            }
            return allBytes;
        }
    }
    private void HandelClient()
    {     
        using (var client = server.AcceptTcpClient())
        {
            Connection();
            string message = Encoding.UTF8.GetString(ReedBytes(client).ToArray());

            // Проверка пароля
            DataBase dataBase = new DataBase();
            LoginPassword loginPassword = new LoginPassword(dataBase);
            int a = -1;
            try
            {
                string[] clientLoginPassword = message.Split('\a');
                a = loginPassword.Login(clientLoginPassword[0], clientLoginPassword[1]);
            }
            catch { Console.WriteLine("Неправильный ввод от компьютера ");}

            if (a > 0)
            { 
                Console.WriteLine("Entered");
            }
            else { }
        }

    }
}


