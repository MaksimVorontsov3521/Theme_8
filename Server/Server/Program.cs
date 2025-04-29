using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Server;
using Server.LocalinfoControl;
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
using System.Xml.XPath;
using System.Text.Json;
using System.Threading.Tasks;

public class Program
{
    string ipAddress = "127.0.0.1";
    int port = Settings1.Default.UserPort;
    TcpListener server;
    static void Main(string[] args)
    {
        //AdminUpdateSetting setting = new AdminUpdateSetting();
        //setting.UpdateBaseFolder("D:\\Desktop\\Root");
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
        UserTable user;
        string[] clientLoginPassword = message.Split('\a');
        try
        {
            user = loginPassword.Login(clientLoginPassword[0], clientLoginPassword[1]);
        }
        catch { Console.WriteLine("Неправильный ввод от компьютера");return; }

        UserSession userSession = new UserSession();
        if (user != null)
        {
            userSession.User = user;
            userSession.clientSocket = clientSocket;
            userSession.Messenger = Messenger;
            userSession.level = loginPassword.GetLevel(userSession.User.RoleID);
            //
            Messenger.SendStrings(clientSocket, "Right\a"+ userSession.level);  

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
        DocumentsAndFolders DAF = new DocumentsAndFolders();
        DBDocumentWork DBD = new DBDocumentWork();
        string success;
        while (true)
        {
            message = Encoding.UTF8.GetString(userSession.Messenger.ReedBytes(userSession.clientSocket));
            switch (message)
            { 
                case "SendPath":
                    message = Encoding.UTF8.GetString(userSession.Messenger.ReedBytes(userSession.clientSocket));
                    byte[] byffer = DAF.ToSendPath(message);
                    userSession.Messenger.SendBytes(userSession.clientSocket, byffer);
                    userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно\n Файлы загружены");
                    break;
                case "GetDocument":
                    string Path = Encoding.UTF8.GetString(userSession.Messenger.ReedBytes(userSession.clientSocket));
                    byte[] document = userSession.Messenger.ReedBytes(userSession.clientSocket);
                    int or = DBD.CanAddNewDocument(Path);
                    switch (or)
                    {
                        case 0:
                            DAF.GetDocument(Path.Split("\a").First(), document);
                            DBD.AddNewDocument(Path, userSession.User.UserLogin);
                            userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно\nНовый файл был добавлен");
                            break;
                        case 1:
                            DAF.GetDocument(Path.Split("\a").First(), document);
                            DBD.RewriteDocument(Path, userSession.User.UserLogin);
                            userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно\nФайл заменён");
                            break;
                        case 2:
                            userSession.Messenger.SendStrings(userSession.clientSocket, "Этот файл нельзя заменить");
                            break;
                    }
                    break;
                case "CreateNewProject":
                    {
                        byte[] Bytes = userSession.Messenger.ReedBytes(userSession.clientSocket);
                        string ProjectName = Encoding.UTF8.GetString(Bytes);

                        if (DBD.GetProjectId(ProjectName) == -1)
                        {
                            userSession.Messenger.SendStrings(userSession.clientSocket, "");
                        }
                        else
                        {
                            userSession.Messenger.SendStrings(userSession.clientSocket, "Проект с таким названием уже существует.\n Возможно проект уже создан другим сотрудником.");
                            break;
                        }

                        Bytes = userSession.Messenger.ReedBytes(userSession.clientSocket);
                        string json = Encoding.UTF8.GetString(Bytes);
                        int[] departmentsIDs = JsonSerializer.Deserialize<int[]>(json);

                        Bytes = userSession.Messenger.ReedBytes(userSession.clientSocket);
                        int patternID = Convert.ToInt32(Encoding.UTF8.GetString(Bytes));

                        Directory.CreateDirectory(Settings1.Default.BaseFolder + "\\" + ProjectName);
                        DBD.NewProject(ProjectName, departmentsIDs, patternID);
                        userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно");

                    }
                    break;
                case "ChangeProjectProperties":
                    {
                        byte[] Bytes = userSession.Messenger.ReedBytes(userSession.clientSocket);
                        string ProjectName = Encoding.UTF8.GetString(Bytes);
                        ProjectName = ProjectName.Remove(ProjectName.Length - 1, 1);
                        ProjectName = ProjectName.Split("\\").Last();

                        if (DBD.GetProjectId(ProjectName) == -1)
                        {
                            userSession.Messenger.SendStrings(userSession.clientSocket, "Что-то не так\nПроекта с таким названием не существует");
                            break;
                        }
                        else
                        {
                            userSession.Messenger.SendStrings(userSession.clientSocket, "");                          
                        }

                        Bytes = userSession.Messenger.ReedBytes(userSession.clientSocket);
                        string json = Encoding.UTF8.GetString(Bytes);
                        int[] departmentsIDs = JsonSerializer.Deserialize<int[]>(json);

                        Bytes = userSession.Messenger.ReedBytes(userSession.clientSocket);
                        int patternID = Convert.ToInt32(Encoding.UTF8.GetString(Bytes));

                        DBD.ChangeProject(ProjectName, departmentsIDs, patternID);
                        userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно");

                    }
                    break;

                case "NewOrUpdateClient":
                    {
                        byte[] jsonBytes = userSession.Messenger.ReedBytes(userSession.clientSocket);
                        string json = Encoding.UTF8.GetString(jsonBytes);
                        UserTable User = JsonSerializer.Deserialize<UserTable>(json);

                        byte[] jsonBytes2 = userSession.Messenger.ReedBytes(userSession.clientSocket);
                        string json2 = Encoding.UTF8.GetString(jsonBytes2);
                        string[] clientProperties = JsonSerializer.Deserialize<string[]>(json2);

                        if (RoleRights.CanEditClient(User.UserLogin, User.UserPassword) == false)
                        {
                            userSession.Messenger.SendStrings(userSession.clientSocket, "У вас нет прав редактировать этот фрагмент");
                            break;
                        }
                        ClientCheck(clientProperties,userSession);

                    }
                    break;

                default:
                    break;
            }
        }
    }

    private async void ClientCheck(string[] clientProperties,UserSession userSession)
    {
        var boolClientNameIsNew = DBClient.IsNew(clientProperties[0]);
        var Availablity = DBClient.ISAvailable(clientProperties);

        var (boolCN, Avai) = await RunBothAsync(boolClientNameIsNew, Availablity);

        if (boolCN == true)
        {
            if (Avai == null)
            {
                DBClient.NewClient(clientProperties);
                userSession.Messenger.SendStrings(userSession.clientSocket, "Новый клиент создан\nУспешно");
            }
            else
            {
                Client client = DBClient.FindClient(clientProperties);
                if (client == null)
                {
                    userSession.Messenger.SendStrings(userSession.clientSocket, Avai);
                }
                else
                {
                    DBClient.UpdateClientName(client, clientProperties[0]);
                    userSession.Messenger.SendStrings(userSession.clientSocket, "Имя клиента обновлено\nУспешно");
                }
            }
        }
        else
        {
            if (Avai == null)
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "Клиент с таким названием уже существует");
            }
            else
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "Этот клиент уже существует");
            }
        }
    }

    public async Task<(T1, T2)> RunBothAsync<T1, T2>(Task<T1> task1, Task<T2> task2)
    {
        await Task.WhenAll(task1, task2);
        return (task1.Result, task2.Result);
    }

    internal void SendTables(UserSession userSession)
    {

        userSession.Messenger.SendJSON(userSession.clientSocket, userSession.User);

        ClientTables clientTables = new ClientTables(userSession.dataBase);

        List<Folder> folder = clientTables.FoldersForClient(userSession.User.DepartmentID);
        userSession.Messenger.SendJSON(userSession.clientSocket, folder);

        List<Pattern> pattern = clientTables.PatternsForClient();
        userSession.Messenger.SendJSON(userSession.clientSocket, pattern);

        List<RequiredInPattern> requiredInPattern = clientTables.RequiredInPatternsForClient();
        userSession.Messenger.SendJSON(userSession.clientSocket, requiredInPattern);

        if (userSession.level <= Settings2.Default.CanCreateNewProject)
        {
            List<Department> departmentsForClient = clientTables.DepartmentsForClient();
            userSession.Messenger.SendJSON(userSession.clientSocket, departmentsForClient);
        }
        else
        {
            List<Department> departmentsForClient = new List<Department> { };
            userSession.Messenger.SendJSON(userSession.clientSocket, departmentsForClient);
        }

    }
    
}


