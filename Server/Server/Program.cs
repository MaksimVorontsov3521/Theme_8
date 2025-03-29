using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Server;
using Server.AdminFolder;
using Server.DataBaseFolder;
using Server.DataBaseFolder.DbContexts;
using Server.DataBaseFolder.Entitys;
using Server.DataBaseFolder.Querys;
using Server.Security;
using Server.Settings;
using System.Net;
using System.Net.Sockets;
using System.Resources;
using System.Text;

public class Program
{
    string ipAddress = "127.0.0.1";
    int port = Server.Settings.Settings1.Default.UserPort;
    TcpListener server;
    static void Main(string[] args)
    {
        AdminUpdateSetting adminUpdateSetting = new AdminUpdateSetting();

        using (var context = new DataBase())
        {
            try
            {
                // Простая проверка подключения
                bool canConnect = context.Database.CanConnect();
                Console.WriteLine(canConnect ? "Подключение успешно!" : "Не удалось подключиться");

                // Альтернативный вариант - выполнить простой запрос
                var test = context.UserTable.FirstOrDefault();
                Console.WriteLine("Подключение работает, запрос выполнен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
                // Для более детальной информации можно проверить InnerException
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }
        Program program = new Program();
        program.Setup();
    }
    private void Setup()
    {
        AdminConnection();

        // Настройки сервера для клиента
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Прослушка
        serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        serverSocket.Listen(10);

        Console.WriteLine("Порт для пользователей запущен");
        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            Thread clientThread = new Thread(() => HandelClient(clientSocket));
            clientThread.Start();
        }

    }
    private void AdminConnection()
    {
        Admin admin = new Admin();
        Thread adminThread = new Thread(() => admin.Connection());
        adminThread.Start();
    }

    private void HandelClient(Socket clientSocket)
    {

        ReadAndWrite Messenger = new ReadAndWrite();
        string message = Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket));

        // Проверка пароля
        DataBase dataBase = new DataBase();
        LoginPassword loginPassword = new LoginPassword(dataBase);
        int a = -1;
        try
        {
            string[] clientLoginPassword = message.Split('\a');
            a = loginPassword.Login(clientLoginPassword[0], clientLoginPassword[1]);
        }
        catch { Console.WriteLine("Неправильный ввод от компьютера"); }

        if (a > 0)
        {
            Messenger.SendStrings(clientSocket, "byte");
            Console.WriteLine("Да");
        }
        else
        {
            Messenger.SendStrings(clientSocket, "no");
            Console.WriteLine("нет");
        }
    }
    
}


