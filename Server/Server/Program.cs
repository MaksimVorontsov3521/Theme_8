﻿using Microsoft.EntityFrameworkCore;
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
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata;
using Azure.Core;
using System.Net.Security;
using System.Security.Authentication;
using System.Configuration;
using System.IO.Compression;
using System.Timers;
using System;

public class Program
{
    string ipAddress = Settings1.Default.ServerUrl;
    int port = Settings1.Default.UserPort;
    private static X509Certificate2 serverCertificate;

    static void Main(string[] args)
    {
        AdminUpdateSetting.CheckSetting();
        CheckDataBase();
        Program program = new Program();
        program.Setup();
    }

    private static void CheckDataBase()
    {
        using (var context = new DataBase())
        {
            try
            {
                // Простая проверка подключения
                bool canConnect = context.Database.CanConnect();
                Console.WriteLine(canConnect ? "Подключение к БД успешно!" : "Не удалось подключиться");

                // простой запрос
                var test = context.UserTable.FirstOrDefault();
                Console.WriteLine("Подключение к БД работает, запрос выполнен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к БД: {ex.Message}");
                // Для более детальной информации можно проверить InnerException
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }
    }
    private void Setup()
    {
        Admin admin = new Admin();
        Thread adminThread = new Thread(() => admin.Connection());
        adminThread.Start();

        // Настройки сервера для клиента
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Прослушка
        serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        serverSocket.Listen(10);

        Console.WriteLine("Порт для пользователей запущен");
        //Приём пользователя
        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            DocumentsAndFolders.isTimeForBackup();
            Thread clientThread = new Thread(() => HandelClient(clientSocket));
            clientThread.Start();
        }
    }
    private async void HandelClient(Socket clientSocket)
    {
        Security security = new Security(clientSocket);
        ReadAndWrite Messenger = new ReadAndWrite(security);
        UserTable user = await CheckPassword(clientSocket, Messenger);
        UserSession userSession = SendUser(user,clientSocket, security);
        if (userSession == null) { return; }
        userSession.Messenger = Messenger;

        SendTables(userSession);
        ClientServerWork(userSession);
    }

    private async static Task<UserTable> CheckPassword(Socket clientSocket, ReadAndWrite Messenger)
    {
        string message = Encoding.UTF8.GetString( await Messenger.ReedBytes(clientSocket));

        // Проверка пароля
        UserTable user = null;
        string[] clientLoginPassword = message.Split('\a');
        try
        {
            user = LoginPassword.Login(clientLoginPassword[0], clientLoginPassword[1]);
            return user;
        }
        catch 
        { 
            Console.WriteLine("Неправильный ввод от компьютера");
            return user; 
        }
    }
    private static UserSession SendUser(UserTable user, Socket clientSocket, Security security)
    {
        ReadAndWrite Messenger = new ReadAndWrite(security);
        UserSession userSession = new UserSession();

        if (user != null)
        {
            userSession.User = user;

            if (user.Blocked == true)
            {
                Messenger.SendStrings(clientSocket, "Вы заблокированы");
                return null;
            }

            userSession.clientSocket = clientSocket;
            userSession.Messenger = Messenger;
            userSession.level = LoginPassword.GetLevel(userSession.User.RoleID);
            //
            Messenger.SendStrings(clientSocket, "Right\a");
            Messenger.SendJSON(clientSocket, LoginPassword.CnaLevel(userSession.level));
            return userSession;
        }
        else
        {
            Messenger.SendStrings(clientSocket, "Неверный логин или пароль");
            return null;
        }
    }


    private async Task ClientServerWork(UserSession userSession)
    {
        bool continueHandleClient = true;
        string message;
        while (continueHandleClient)
        {
            message = Encoding.UTF8.GetString( await userSession.Messenger.ReedBytes(userSession.clientSocket));
            switch (message)
            { 
                    // Скачать файл на ПК пользователя
                case "SendPath":
                    ClientServerWorkClass.SendPath(message,userSession);
                    break;
                    // Добавить или заменить файл проекта
                case "GetDocument":
                    ClientServerWorkClass.GetDocument(message, userSession);
                    break;
                    // Создать новый проект
                case "CreateNewProject":
                    ClientServerWorkClass.CreateNewProject(userSession);
                    break;
                    // Изменить проект
                case "ChangeProjectProperties":
                    ClientServerWorkClass.ChangeProjectProperties(userSession);
                    break;
                    // Добавить или обновить клиента
                case "NewOrUpdateClient":
                    ClientServerWorkClass.NewOrUpdateClient(userSession);
                    break;
                    // Найти клиента
                case "FindClient":
                    ClientServerWorkClass.FindClient(userSession);
                    break;
                    // Продолжить проект, изменить состояние проекта
                case "ContinueProject":
                    ClientServerWorkClass.ContinueProject(userSession);
                    break;
                default:                   
                    continueHandleClient = false;
                    break;
            }
        }
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