using Client.Pages;
using Client.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Text.Json;
using Client.Classes;
using System.Reflection.Metadata;
using Server.DataBaseFolder.Entitys;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Shapes;
using System.IO.Pipes;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Client
{
    public class Server
    {
        MainWindow mainWindow;
        ReadAndWrite Messenger = new ReadAndWrite();
        MainWorkPage page;

        Socket clientSocket;
        string serverIP = Settings1.Default.ServerURL;
        int port = Settings1.Default.ServerPort;

        internal List<ServerDocument> receivedDocuments = new List<ServerDocument>();
        internal List<Folder> receivedFolders = new List<Folder>();
        internal List<Pattern> receivedPatterns = new List<Pattern>();
        internal List<RequiredInPattern> receivedRequiredInPatterns = new List<RequiredInPattern>();

        private string[] LoginPassword = new string[2];

        public Server(MainWindow mainWindow)
        {
            // Связка окна и сервера
            this.mainWindow = mainWindow;
            Server server = this;
            Entrance entrance = new Entrance(server);
            mainWindow.WorkPlace.Navigate(entrance);
            page = new MainWorkPage(this);
        }

        public void Connection(string login, string password)
        {

            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(IPAddress.Parse(serverIP), port);
            }
            catch
            {
                MessageBox.Show("Сервер недоступен");
                return;
            }


            Messenger.SendStrings(clientSocket, login + "\a" + password + "\a");

            if (Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)).Contains("Right"))
            { }
            else { MessageBox.Show("Неверный логин или пароль");return; }

            LoginPassword[0]=login;
            LoginPassword[1]=password;

            GetTables();
            CreateWorkPlace();
        }

        public void CreateWorkPlace()
        {           
            for (int i = 0; i < receivedFolders.Count; i++)
            {
                page.ProjectsListBox.Items.Add(receivedFolders[i].FolderPath);
                page.FindProjectComboBox.Items.Add(receivedFolders[i].FolderPath);
            }            
            mainWindow.MainWorkPageNavigate(page);
        }

        public void UpdateDocuments(int FolderCount)
        {
            int folderID = receivedFolders[FolderCount].FolderID;

            page.DocumentsListBox.Items.Clear();
            for (int i = 0; i < receivedDocuments.Count; i++)
            {
                if (receivedDocuments[i].FolderID == folderID)
                {
                    page.DocumentsListBox.Items.Add(receivedDocuments[i].DocumentName);
                }
            }
        }

        public void GetTables()
        {
            byte[] jsonBytes = Messenger.ReedBytes(clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            receivedFolders = JsonSerializer.Deserialize<List<Folder>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedDocuments = JsonSerializer.Deserialize<List<ServerDocument>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedPatterns = JsonSerializer.Deserialize<List<Pattern>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedRequiredInPatterns = JsonSerializer.Deserialize<List<RequiredInPattern>>(json);
        }

        public void DownloadDocument(string ServerFileRoot, string FileName)
        {
            Messenger.SendStrings(clientSocket,"SendPath");
            Messenger.SendStrings(clientSocket, ServerFileRoot+ FileName);
            byte[] document = Messenger.ReedBytes(clientSocket);

            string[] projectName = page.ProjectsListBox.SelectedItem.ToString().Split("\\");
            Directory.CreateDirectory(Settings1.Default.RootFolder + "\\" + projectName[projectName.Length - 2]);
            string PathToSave = Settings1.Default.RootFolder+"\\"+ projectName[projectName.Length-2] +"\\"+ FileName;

            using (FileStream writer = new FileStream(PathToSave, FileMode.Create))
            {
                writer.Write(document, 0, document.Length);
            }
            TransactionResult(Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)));
        }

        public void SendDocument(string ProjectName, string FileName)
        {
            Messenger.SendStrings(clientSocket, "GetDocument");
            string[] FileParts = FileName.Split('\\');
            Messenger.SendStrings(clientSocket, ProjectName + "\\" + FileParts[FileParts.Length-1]);
            byte[] bytes = File.ReadAllBytes(FileName);
            Messenger.SendBytes(clientSocket, bytes);

            TransactionResult(Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)));
            if (page.DocumentsListBox.Items.IndexOf(FileParts[FileParts.Length - 1])!=-1)
            { 
            return;
            }
            
            ServerDocument serverDocument = new ServerDocument();
            serverDocument.DocumentName = FileParts[FileParts.Length - 1];
            serverDocument.IsDone = true;
            serverDocument.FolderID = receivedFolders[page.ProjectsListBox.SelectedIndex].FolderID;
 
            receivedDocuments.Add(serverDocument);
            UpdateDocuments(page.ProjectsListBox.SelectedIndex);
        }

        internal string[] FindFolder(string Name)
        {
            var matching = receivedFolders.Where(f => f.FolderPath.Contains(Name)).ToList();
            string[] folders = new string[matching.Count];
            for (int i = 0; i < folders.Length; i++)
            {
                folders[i] = matching[i].FolderPath;
            }
            return folders;
        }

        internal void TransactionResult(string Result)
        {
            
            if (Result.Contains("Успешно"))
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightGreen);           
            }
            else
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightPink);
            }

            // Таймер для запуска анимации через 2 секунды
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (sender, e) =>
            {
                page.Popup1.IsOpen = false;
                timer.Stop();
            };
            timer.Start();

        }
    }
}
