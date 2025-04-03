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
using Server.Session;
using Server.Settings;
using System.Diagnostics;
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
        Console.WriteLine();
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
        string[] clientLoginPassword = message.Split('\a');
        try
        {            
            a = loginPassword.Login(clientLoginPassword[0], clientLoginPassword[1]);
        }
        catch { Console.WriteLine("Неправильный ввод от компьютера");return; }

        UserSession userSession = new UserSession();
        if (a > 1)
        {
            Messenger.SendStrings(clientSocket, "Right");
            //      
            userSession.userLogin = clientLoginPassword[0];
            userSession.clientSocket = clientSocket;
            userSession.Messenger = Messenger;
        }
        else
        {
            Messenger.SendStrings(clientSocket, "Wrong");
            return;
        }
        SendTables(userSession);
        ClientServerWork(userSession);
    }

    private void ClientServerWork(UserSession userSession)
    {
        string message;
        Server.LocalinfoControl.DocumentsAndFolders DAF = new Server.LocalinfoControl.DocumentsAndFolders();
        while (true)
        {
            message = Encoding.UTF8.GetString(userSession.Messenger.ReedBytes(userSession.clientSocket));
            switch (message)
            { 
                case "SendPath":
                    message = Encoding.UTF8.GetString(userSession.Messenger.ReedBytes(userSession.clientSocket));
                    byte[] byffer = DAF.ToSendPath(message);
                    userSession.Messenger.SendBytes(userSession.clientSocket, byffer);
                    break;
                    default:
                    break;
            }
        }
    }

    internal void SendTables(UserSession userSession)
    {

        ClientTables clientTables = new ClientTables(userSession.dataBase);
        List<Document> documents = clientTables.DocumentsForClient();
        userSession.Messenger.SendJSON( userSession.clientSocket, documents);

        List<Folder> folder = clientTables.FoldersForClient(userSession.userLogin);
        userSession.Messenger.SendJSON(userSession.clientSocket, folder);

        List<Pattern> pattern = clientTables.PatternsForClient();
        userSession.Messenger.SendJSON(userSession.clientSocket, pattern);

        List<RequiredInPattern> requiredInPattern = clientTables.RequiredInPatternsForClient();
        userSession.Messenger.SendJSON(userSession.clientSocket, requiredInPattern);
    }
    
}


