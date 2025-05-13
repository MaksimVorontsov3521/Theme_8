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
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Shapes;
using System.IO.Pipes;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using Client.Resources.Entitys;
using System.Windows.Documents;
using System.Collections;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using System.Diagnostics;
using Client.Classes.Visual;

namespace Client
{
    public class Server
    {
        MainWindow mainWindow;
        MainWorkPage page;

        ReadAndWrite Messenger;

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

        public async Task Connection(string login, string password)
        {        
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await clientSocket.ConnectAsync(IPAddress.Parse(serverIP), port);
                ReadAndWrite messenger = new ReadAndWrite(clientSocket);
                Messenger = messenger;
            }
            catch
            {
                MessageBox.Show("Сервер недоступен");
                return;
            }
            await Messenger.SendStrings(clientSocket, login + "\a" + password + "\a");
            string response = Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket));
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
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(session.receivedFolders[i].FolderPath));
                textBlock.Inlines.Add(new Run("\n"+session.receivedFolders[i].Client.ClientName) { FontWeight = FontWeights.Bold });
                page.ProjectsListBox.Items.Add(textBlock);
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
            page.SortBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public void TimeSort()
        {
            StyleClass.TimeSort(session, page);
        }
        public void TimeSortReversed()
        {
            StyleClass.TimeSortReversed(session, page);
        }
        public void NameSort()
        {
            StyleClass.NameSort(session,page);
        }
        public void NameSortReversed()
        {
            StyleClass.NameSortReversed(session,page);
        }
        public void ClientSort()
        {
            StyleClass.ClientSort(session,page);
        }
        public void ClientSortReversed()
        {
            StyleClass.ClientSortReversed(session,page);
        }
        public void UnDone()
        {
            StyleClass.UnDone(session,page);
        }

        //

        public void UpdateDocuments(int FolderCount)
        {
            ShowDocuments showDocuments = new ShowDocuments(session, page);
            showDocuments.UpdateDocuments(FolderCount);
        }

        public async void GetTables()
        {
            byte[] jsonBytes = await Messenger.ReedBytes(clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            session.thisUser = JsonSerializer.Deserialize<ThisUser>(json);

            jsonBytes = await Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            session.receivedFolders = JsonSerializer.Deserialize<List<Folder>>(json);

            jsonBytes = await Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            session.receivedPatterns = JsonSerializer.Deserialize<List<Pattern>>(json);

            jsonBytes = await Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            session.receivedRequiredInPatterns = JsonSerializer.Deserialize<List<RequiredInPattern>>(json);

            jsonBytes = await Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            session.department = JsonSerializer.Deserialize<List<Department>>(json);
        }


        public async void DownloadDocument(object ServerFileRoot, string FileName)
        {
            if (FileName == "")
            {
                StyleClass.TransactionResult("Данный документ ещё не прикреплён",page);
                return;
            }

            string folderPath = "";
            if (ServerFileRoot is TextBlock selectedTextBlock)
            {
                foreach (var inline in selectedTextBlock.Inlines)
                {
                    if (inline is Run run)
                    {
                        folderPath = ((Run)selectedTextBlock.Inlines.FirstInline).Text;
                    }
                }
            }
            else
            {
                StyleClass.TransactionResult("Что-то пошло не так", page);
                return;
            }

            await Messenger.SendStrings(clientSocket, "SendPath");
            await Messenger.SendStrings(clientSocket, folderPath + FileName);


            byte[] document = await Messenger.ReedBytes(clientSocket);

            string[] projectName = folderPath.Split("\\");
            Directory.CreateDirectory(Settings1.Default.RootFolder + "\\" + projectName[projectName.Length - 2]);
            string PathToSave = Settings1.Default.RootFolder+"\\"+ projectName[projectName.Length-2] +"\\"+ FileName;

            using (FileStream writer = new FileStream(PathToSave, FileMode.Create))
            {
                writer.WriteAsync(document, 0, document.Length);
            }

            StyleClass.TransactionResult(Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket)),page);
        }

        public async Task CreateNewProject(string[] departments, int patternID, string ProjectName)
        {
            int[] departmentsIDs = new int[departments.Length];
            for (int i = 0; i < departments.Length; i++)
            {
                departmentsIDs[i]=Convert.ToInt32(departments[i].Split("\a").First());
            }
            await Messenger.SendStrings(clientSocket, "CreateNewProject");
            await Messenger.SendStrings(clientSocket, ProjectName);

            string Unique= Encoding.UTF8.GetString( await Messenger.ReedBytes(clientSocket));
            if (Unique != "") { StyleClass.TransactionResult(Unique, page);return; }

            await Messenger.SendJSON(clientSocket, departmentsIDs);
            await Messenger.SendStrings(clientSocket, patternID.ToString());
            StyleClass.TransactionResult(Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket)), page);
        }

        public async void ChangeProjectProperties(string[] departments, int patternID, string ProjectName)
        {
            int[] departmentsIDs = new int[departments.Length];
            for (int i = 0; i < departments.Length; i++)
            {
                departmentsIDs[i] = Convert.ToInt32(departments[i].Split("\a").First());
            }
            await Messenger.SendStrings(clientSocket, "ChangeProjectProperties");
            await Messenger.SendStrings(clientSocket, ProjectName);

            string Unique = Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket));
            if (Unique != "") { StyleClass.TransactionResult(Unique, page); return; }

            await Messenger.SendJSON(clientSocket, departmentsIDs);
            await Messenger.SendStrings(clientSocket, patternID.ToString());
            StyleClass.TransactionResult(Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket)), page);
        }

        public async void SendDocument(object ProjectName, string FileName,int nameInPattern)
        {
            //
            //
            //

            string folderPath = "";
            if (ProjectName is TextBlock selectedTextBlock)
            {               
                foreach (var inline in selectedTextBlock.Inlines)
                {
                    if (inline is Run run)
                    {
                       folderPath = ((Run)selectedTextBlock.Inlines.FirstInline).Text;
                    }
                }
            }
            else 
            {
                StyleClass.TransactionResult("Что-то пошло не так", page);
                return;
            }

            //
            //
            //
            FileName = FileName.Trim(new char[] { '\n', '\r' });
            var folder = session.receivedFolders.FirstOrDefault(f => f.FolderPath == folderPath);
            var Doc = folder.Documents.FirstOrDefault(d => d.DocumentName ==  FileName.Split("\\").Last());
            if (Doc!=null)
            {
                if (Doc.InPatternID != null)
                { nameInPattern =Convert.ToInt32(Doc.InPatternID);}              
            }

            await Messenger.SendStrings(clientSocket, "GetDocument");
            string[] FileParts = FileName.Split('\\');

            await Messenger.SendStrings(clientSocket, folderPath + "\\" + FileParts[FileParts.Length - 1] + "\a" + nameInPattern);
            byte[] bytes = File.ReadAllBytes(FileName);
            await Messenger.SendBytes(clientSocket, bytes);
            
            string result = Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket));
            StyleClass.TransactionResult(result, page);
            if (page.DocumentsListBox.Items.IndexOf(FileParts[FileParts.Length - 1])!=-1)
            { 
            return;
            }

            var notUniqueDoc = folder.Documents.FirstOrDefault(d=> d.InPatternID== nameInPattern);
            if (notUniqueDoc != null)
            {
                notUniqueDoc.InPatternID = null;
            }


            if (result.Contains("Новый"))
            {
                ServerDocument serverDocument = new ServerDocument();
                serverDocument.DocumentName = FileParts[FileParts.Length - 1];
                serverDocument.InPatternID = nameInPattern;
                serverDocument.FolderID = session.receivedFolders[page.ProjectsListBox.SelectedIndex].FolderID;
                session.receivedFolders[page.ProjectsListBox.SelectedIndex].Documents.Add(serverDocument);
            }
            else {
                List<ServerDocument> Documents = session.receivedFolders[page.ProjectsListBox.SelectedIndex].Documents;
                ServerDocument serverDocument = Documents.First(d => d.DocumentName == FileParts[FileParts.Length - 1]);
                int index = Documents.FindIndex((d => d.DocumentName == FileParts[FileParts.Length - 1]));
                if (serverDocument != null)
                {
                    serverDocument.InPatternID = nameInPattern;
                    session.receivedFolders[page.ProjectsListBox.SelectedIndex].Documents[index]= serverDocument;
                }           
            }
            IsProjectDone(page.ProjectsListBox.SelectedIndex);
            UpdateDocuments(page.ProjectsListBox.SelectedIndex);
        }

        public void IsProjectDone(int ProjectID)
        {
            int howManyPattern = 0;
            int patternID = 0;
            if (session.receivedFolders[ProjectID].PatternID != null)
            {
                patternID = Convert.ToInt32(session.receivedFolders[ProjectID].PatternID);
            }
            else { return; }
            patternID--;

            if (patternID >= 0)
            {
                List<RequiredInPattern> NeedInFolderObj = session.receivedPatterns[patternID].RequiredInPatterns;
                List<string> NeedInFolderStr = new List<string>();
                for (int i = 0; i < NeedInFolderObj.Count; i++)
                {
                    var doc = session.receivedFolders[ProjectID].Documents.FirstOrDefault(d => d.InPatternID == NeedInFolderObj[i].DocumentPatternID);
                    if (doc == null)
                    {
                        NeedInFolderStr.Add(NeedInFolderObj[i].DocumentName);
                    }
                }

                for (int i = 0; i < NeedInFolderStr.Count; i++)
                {
                    howManyPattern++;
                }
            }

            if(howManyPattern==0)
            {
                CompleteProject(ProjectID);
                StyleClass.TransactionResult("Проект Успешно завершён", page);
            }        
        }

        public string[] FindFolder(string Name)
        {
            var matching = session.receivedFolders.Where(f => f.FolderPath.Contains(Name)).ToList();
            string[] folders = new string[matching.Count];
            for (int i = 0; i < folders.Length; i++)
            {
                folders[i] = matching[i].FolderPath;
            }
            return folders;
        }

        public int GetNameInPatternID(int projectIndex,string nameInPattern)
        {
            int pID = Convert.ToInt32(session.receivedFolders[projectIndex].PatternID);
            List<RequiredInPattern> requiredInPatterns = session.receivedPatterns[pID-1].RequiredInPatterns;

            RequiredInPattern filteredItems = requiredInPatterns.OfType<RequiredInPattern>()
                          .FirstOrDefault(x => x.DocumentName == nameInPattern);
            return filteredItems.DocumentPatternID;
        }

        public Folder GetFolderByName(string folderName)
        {
            Folder folder = session.receivedFolders.FirstOrDefault(f=> f.FolderPath==folderName);
            return folder;
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

        public async void NewOrUpdateClient(string[]clientInfo)
        {
            await Messenger.SendStrings(clientSocket, "NewOrUpdateClient");
            await Messenger.SendJSON(clientSocket, session.thisUser);

            await Messenger.SendJSON(clientSocket, clientInfo);

            string result = Encoding.UTF8.GetString( await Messenger.ReedBytes(clientSocket));
            StyleClass.TransactionResult(result, page);
        }

        public async void FindClient(string ClientName)
        {
            await Messenger.SendStrings(clientSocket, "FindClient");
            await Messenger.SendJSON(clientSocket, session.thisUser);
            await Messenger.SendStrings(clientSocket, ClientName);

            string result = Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket));
            string [] clientInfo = result.Split('\b');           

            result = Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket));
            StyleClass.TransactionResult(result, page);

            if (clientInfo.Length >= 5)
            {
                page.INNLabel.Content = clientInfo[1];
                page.EmailLabel.Content = clientInfo[2];
                page.OGRNLabel.Content = clientInfo[3];
                page.KPPLabel.Content = clientInfo[4];
            }
        }

        public void ContinueProject(int ProjectID)
        {
            Folder folder = session.receivedFolders[ProjectID];
            if (folder.IsDone == false)
            {
                StyleClass.TransactionResult("Проект уже активен", page);
                return;
            }
            folder.IsDone = false;
            ChangeProject(folder);
        }

        public void CompleteProject(int ProjectID)
        {
            Folder folder = session.receivedFolders[ProjectID];
            if (folder.IsDone == true)
            {
                StyleClass.TransactionResult("Проект уже завершён", page);
                return;
            }
            folder.IsDone = true;
            ChangeProject(folder);
        }

        private async void ChangeProject(Folder folder)
        {           
            await Messenger.SendStrings(clientSocket, "ContinueProject");
            await Messenger.SendJSON(clientSocket, folder);
            string a = Encoding.UTF8.GetString(await Messenger.ReedBytes(clientSocket));
            StyleClass.TransactionResult(a, page);
            page.SortBox_SelectionChanged();
        }


    }
}
