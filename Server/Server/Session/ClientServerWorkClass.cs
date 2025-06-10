using Server.DataBaseFolder.Entitys;
using Server.DataBaseFolder.Querys;
using Server.LocalinfoControl;
using Server.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Session
{
    internal class ClientServerWorkClass : Program
    {
        // Скачать файл на ПК пользователя
        public static async Task SendPath(string message, UserSession userSession)
        {
            message = Encoding.UTF8.GetString(await userSession.Messenger.ReedBytes(userSession.clientSocket));
            
            if (DBDocumentWork.IsFolderExist(message, userSession) == false)
            {
                byte[] Error = new byte[1];
                userSession.Messenger.SendBytes(userSession.clientSocket, Error);
                userSession.Messenger.SendStrings(userSession.clientSocket, "Файла с таким именем не существует");
                return;
            }

            byte[] byffer = DocumentsAndFolders.ToSendPath(message);
            userSession.Messenger.SendBytes(userSession.clientSocket, byffer);
            userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно\n Файлы загружены");
        }
        // Добавить или заменить файл проекта
        public static async Task GetDocument(string message, UserSession userSession)
        {
            string Path = Encoding.UTF8.GetString(await userSession.Messenger.ReedBytes(userSession.clientSocket));
            byte[] document = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            int or = DBDocumentWork.CanAddNewDocument(Path);
            switch (or)
            {
                case 0:
                    DocumentsAndFolders.GetDocument(Path.Split("\a").First(), document);
                    DBDocumentWork.AddNewDocument(Path, userSession.User.UserLogin);
                    userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно\nНовый файл был добавлен");
                    break;
                case 1:
                    DocumentsAndFolders.GetDocument(Path.Split("\a").First(), document);
                    DBDocumentWork.RewriteDocument(Path, userSession.User.UserLogin);
                    userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно\nФайл заменён");
                    break;
                case 2:
                    userSession.Messenger.SendStrings(userSession.clientSocket, "Этот файл нельзя заменить");
                    break;
            }
        }
        //
        public static async Task CreateNewProject(UserSession userSession)
        {
            byte[] Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string ProjectName = Encoding.UTF8.GetString(Bytes);

            if (DBDocumentWork.GetProjectId(ProjectName) == -1)
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "");
            }
            else
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "Проект с таким названием уже существует.\n Возможно проект уже создан другим сотрудником.");
                return;
            }

            Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string client = Encoding.UTF8.GetString(Bytes);

            if (DBDocumentWork.IsAClient(client) == true)
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "+");
            }
            else
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "Такого клиента не существует");
                return;
            }

            Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string json = Encoding.UTF8.GetString(Bytes);
            int[] departmentsIDs = JsonSerializer.Deserialize<int[]>(json);

            Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            int patternID = Convert.ToInt32(Encoding.UTF8.GetString(Bytes));


            Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            json = Encoding.UTF8.GetString(Bytes);
            DateTime DeadLine = JsonSerializer.Deserialize<DateTime>(json);

            Directory.CreateDirectory(Settings1.Default.BaseFolder + "\\" + ProjectName);
            DBDocumentWork.NewProject(ProjectName, departmentsIDs, patternID,DeadLine,client);
            userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно");
        }
        //
        public static async void ChangeProjectProperties(UserSession userSession)
        {
            byte[] Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string ProjectName = Encoding.UTF8.GetString(Bytes);
            ProjectName = ProjectName.Remove(ProjectName.Length - 1, 1);
            ProjectName = ProjectName.Split("\\").Last();

            if (DBDocumentWork.GetProjectId(ProjectName) == -1)
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "Что-то не так\nПроекта с таким названием не существует");
                return;
            }
            else
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "");
            }

            Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string json = Encoding.UTF8.GetString(Bytes);
            int[] departmentsIDs = JsonSerializer.Deserialize<int[]>(json);

            Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            int patternID = Convert.ToInt32(Encoding.UTF8.GetString(Bytes));

            Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            json = Encoding.UTF8.GetString(Bytes);
            DateTime DeadLine = JsonSerializer.Deserialize<DateTime>(json);

            DBDocumentWork.ChangeProject(ProjectName, departmentsIDs, patternID,DeadLine);
            userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно");
        }
        //
        public static async void NewOrUpdateClient(UserSession userSession)
        {
            byte[] jsonBytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            UserTable User = JsonSerializer.Deserialize<UserTable>(json);

            byte[] jsonBytes2 = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string json2 = Encoding.UTF8.GetString(jsonBytes2);
            string[] clientProperties = JsonSerializer.Deserialize<string[]>(json2);

            if (RoleRights.CanEditClient(User.UserLogin, User.UserPassword) == false)
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "У вас нет прав редактировать этот фрагмент");
                return;
            }
            ClientCheck(clientProperties, userSession);
        }
        //
        private static async void ClientCheck(string[] clientProperties, UserSession userSession)
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

        //
        public static async Task<(T1, T2)> RunBothAsync<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            await Task.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }

        //

        public static async void FindClient(UserSession userSession)
        {
            byte[] jsonBytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            UserTable User = JsonSerializer.Deserialize<UserTable>(json);

            byte[] Bytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string stringByte = Encoding.UTF8.GetString(Bytes);
            Client client = new Client();
            if (RoleRights.CanEditClient(User.UserLogin, User.UserPassword) == false)
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "");
                userSession.Messenger.SendStrings(userSession.clientSocket, "У вас нет прав редактировать этот фрагмент");
                return;
            }
            client = DBClient.FindClientByName(stringByte);
            if (client == null)
            {
                userSession.Messenger.SendStrings(userSession.clientSocket, "");
                userSession.Messenger.SendStrings(userSession.clientSocket, "Нет клиента с таким именем");
                return;

            }
            string clientInfo = client.ClientName + "\b" +
                client.INN + "\b" + client.Email + "\b" + client.OGRN + "\b" + client.KPP;

            userSession.Messenger.SendStrings(userSession.clientSocket, clientInfo);
            userSession.Messenger.SendStrings(userSession.clientSocket, "Успешно");
        }

        //
        public static async void ContinueProject(UserSession userSession)
        {
            byte[] jsonBytes = await userSession.Messenger.ReedBytes(userSession.clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            Folder folder = JsonSerializer.Deserialize<Folder>(json);
            DBDocumentWork.UpdateFolder(folder);
            userSession.Messenger.SendStrings(userSession.clientSocket, "Изменено успешно");
        }


    }
}
