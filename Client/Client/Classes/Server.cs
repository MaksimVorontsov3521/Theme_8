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
using Client.Resources.Entitys;

namespace Client
{
    public class Server
    {
        MainWindow mainWindow;
        ReadAndWrite Messenger = new ReadAndWrite();
        MainWorkPage page;

        Session session;
        Socket clientSocket;
        string serverIP = Settings1.Default.ServerURL;
        int port = Settings1.Default.ServerPort;



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
            string response = Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket));
            if (response.Contains("Right"))
            {
                session = new Session();
                session.level = Convert.ToInt32(response.Split("\a").Last());
                LoginPassword[0] = login;
                LoginPassword[1] = password;

            }
            else { MessageBox.Show("Неверный логин или пароль");return; }
            GetTables();
            CreateWorkPlace();
        }

        public void CreateWorkPlace()
        {           
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                page.ProjectsListBox.Items.Add(session.receivedFolders[i].FolderPath);
                page.FindProjectComboBox.Items.Add(session.receivedFolders[i].FolderPath);
            }
            for (int i = 0; i < session.department.Count; i++)
            {
                page.AllDepartments.Items.Add(session.department[i].DepartmentID +"\a "+session.department[i].DepartmentName);
                page.AllDepartmentsNew.Items.Add(session.department[i].DepartmentID + "\a " + session.department[i].DepartmentName);
            }
            for (int i = 0; i < session.receivedPatterns.Count; i++)
            {
                page.AplyedNewProjectPattern.Items.Add(session.receivedPatterns[i].PatternName);
                page.AplyedProjectPattern.Items.Add(session.receivedPatterns[i].PatternName);
            }
            mainWindow.MainWorkPageNavigate(page);
        }

        public void UpdateDocuments(int FolderCount)
        {
            int howManyDocs = session.receivedFolders[FolderCount].Documents.Count;
            page.DocumentsListBox.Items.Clear();

            for (int i = 0; i < howManyDocs; i++)
            {
                page.DocumentsListBox.Items.Add(session.receivedFolders[FolderCount].Documents[i].DocumentName);
            }
        }

        public void GetTables()
        {
            byte[] jsonBytes = Messenger.ReedBytes(clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            session.thisUser = JsonSerializer.Deserialize<ThisUser>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            session.receivedFolders = JsonSerializer.Deserialize<List<Folder>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            session.receivedPatterns = JsonSerializer.Deserialize<List<Pattern>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            session.receivedRequiredInPatterns = JsonSerializer.Deserialize<List<RequiredInPattern>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            session.department = JsonSerializer.Deserialize<List<Department>>(json);
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

        internal void CreateNewProject(string[] departments, int patternID, string ProjectName)
        {
            int[] departmentsIDs = new int[departments.Length];
            for (int i = 0; i < departments.Length; i++)
            {
                departmentsIDs[i]=Convert.ToInt32(departments[i].Split("\a").First());
            }
            Messenger.SendStrings(clientSocket, "CreateNewProject");
            Messenger.SendStrings(clientSocket, ProjectName);

            string Unique= Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket));
            if (Unique != "") { TransactionResult(Unique);return; }


            Messenger.SendJSON(clientSocket, departmentsIDs);
            Messenger.SendStrings(clientSocket, patternID.ToString());
            TransactionResult(Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)));
        }

        internal void ChangeProjectProperties(string[] departments, int patternID, string ProjectName)
        {
            int[] departmentsIDs = new int[departments.Length];
            for (int i = 0; i < departments.Length; i++)
            {
                departmentsIDs[i] = Convert.ToInt32(departments[i].Split("\a").First());
            }
            Messenger.SendStrings(clientSocket, "ChangeProjectProperties");
            Messenger.SendStrings(clientSocket, ProjectName);

            string Unique = Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket));
            if (Unique != "") { TransactionResult(Unique); return; }

            Messenger.SendJSON(clientSocket, departmentsIDs);
            Messenger.SendStrings(clientSocket, patternID.ToString());
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
            serverDocument.FolderID = session.receivedFolders[page.ProjectsListBox.SelectedIndex].FolderID;
            session.receivedFolders[page.ProjectsListBox.SelectedIndex].Documents.Add(serverDocument);

            UpdateDocuments(page.ProjectsListBox.SelectedIndex);
        }

        internal string[] FindFolder(string Name)
        {
            var matching = session.receivedFolders.Where(f => f.FolderPath.Contains(Name)).ToList();
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

        public bool IsDocumentNew(int FolderCount, string docName)
        {
            docName= docName.Split('\\').Last();
            int howManyDocs = session.receivedFolders[FolderCount].Documents.Count;
            for (int i = 0; i < howManyDocs; i++)
            {
                if (session.receivedFolders[FolderCount].Documents[i].DocumentName == docName)
                {
                    return false; ;
                }
            }
            return true;
        }
    }
}
