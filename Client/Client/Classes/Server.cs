﻿using Client.Pages;
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
using System.Windows.Documents;
using System.Collections;

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
            var Folder = session.receivedFolders.Cast<Folder>().OrderBy(f => f.FolderID).ToList();          
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(Folder[i].FolderPath));
                textBlock.Inlines.Add(new Run("\n" + Folder[i].Client.ClientName) { FontWeight = FontWeights.Bold });

                if (Folder[i].IsDone == false)
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Red });
                }
                else
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Green });
                }

                page.ProjectsListBox.Items.Add(textBlock);
                page.ProjectsListBox.Items.RemoveAt(0);
                session.receivedFolders = Folder;
            }  
        }
        public void TimeSortReversed()
        {
            var Folder = session.receivedFolders.Cast<Folder>().OrderByDescending(f => f.FolderID).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(Folder[i].FolderPath));
                textBlock.Inlines.Add(new Run("\n" + Folder[i].Client.ClientName) { FontWeight = FontWeights.Bold });

                if (Folder[i].IsDone == false)
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Red });
                }
                else
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Green });
                }

                page.ProjectsListBox.Items.Add(textBlock);
                page.ProjectsListBox.Items.RemoveAt(0);
                session.receivedFolders = Folder;
            }
        }
        public void NameSort()
        {
            var Folder = session.receivedFolders.Cast<Folder>().OrderBy(f => f.FolderPath).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(Folder[i].FolderPath));
                textBlock.Inlines.Add(new Run("\n" + Folder[i].Client.ClientName) { FontWeight = FontWeights.Bold });

                if (Folder[i].IsDone == false)
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Red });
                }
                else
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Green });
                }

                page.ProjectsListBox.Items.Add(textBlock);
                page.ProjectsListBox.Items.RemoveAt(0);
                session.receivedFolders = Folder;
            }
        }
        public void NameSortReversed()
        {
            var Folder = session.receivedFolders.Cast<Folder>().OrderByDescending(f => f.FolderPath).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(Folder[i].FolderPath));
                textBlock.Inlines.Add(new Run("\n" + Folder[i].Client.ClientName) { FontWeight = FontWeights.Bold });

                if (Folder[i].IsDone == false)
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Red });
                }
                else
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Green });
                }

                page.ProjectsListBox.Items.Add(textBlock);
                page.ProjectsListBox.Items.RemoveAt(0);
                session.receivedFolders = Folder;
            }
        }

        public void ClientSort()
        {
            var Folder = session.receivedFolders.Cast<Folder>().OrderBy(f => f.Client.ClientName).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(Folder[i].FolderPath));
                textBlock.Inlines.Add(new Run("\n" + Folder[i].Client.ClientName) { FontWeight = FontWeights.Bold });

                if (Folder[i].IsDone == false)
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Red });
                }
                else
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Green });
                }

                page.ProjectsListBox.Items.Add(textBlock);
                page.ProjectsListBox.Items.RemoveAt(0);
                session.receivedFolders = Folder;
            }
        }

        public void ClientSortReversed()
        {
            var Folder = session.receivedFolders.Cast<Folder>().OrderByDescending(f => f.Client.ClientName).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(Folder[i].FolderPath));
                textBlock.Inlines.Add(new Run("\n" + Folder[i].Client.ClientName) { FontWeight = FontWeights.Bold });

                if (Folder[i].IsDone == false)
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Red });
                }
                else
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Green });
                }

                page.ProjectsListBox.Items.Add(textBlock);
                page.ProjectsListBox.Items.RemoveAt(0);
                session.receivedFolders = Folder;
            }
        }

        public void UnDone()
        {
            var Folder = session.receivedFolders.Cast<Folder>().OrderBy(f => f.IsDone).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(new Run(Folder[i].FolderPath));
                textBlock.Inlines.Add(new Run("\n" + Folder[i].Client.ClientName) { FontWeight = FontWeights.Bold });

                if (Folder[i].IsDone == false)
                {
                    textBlock.Inlines.Add(new Run("\a   " + Folder[i].DeadLine));
                    textBlock.Inlines.Add(new Run("\a   " + "●") { Foreground = Brushes.Red });
                }
                else
                {
                    textBlock.Inlines.Add(new Run("\a" + "●") { Foreground = Brushes.Green });
                }

                page.ProjectsListBox.Items.Add(textBlock);
                page.ProjectsListBox.Items.RemoveAt(0);
                session.receivedFolders = Folder;
            }
        }

        public void UpdateDocuments(int FolderCount)
        {
            if (FolderCount == -1) { return; }
            int howManyDocs = session.receivedFolders[FolderCount].Documents.Count;
            page.DocumentsListBox.Items.Clear();
            UpdateDocumentsFile(howManyDocs, FolderCount);               
        }

        public void UpdateDocumentsFile(int howManyDocs, int FolderCount)
        {
            int n= page.InPatternBox.SelectedIndex;
            page.DocumentsListBox.Items.Clear();
            switch (n)
            { 
                case 1:
                    FileTimeSort(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    UpdateDocumentsPattern(FolderCount);
                    break;
                case 2:
                    FileTimeSortReversed(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    UpdateDocumentsPattern(FolderCount);
                    break;
                case 3:
                    FileNameSort(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    UpdateDocumentsPattern(FolderCount);
                    break;
                case 4:
                    FileNameSortReversed(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    UpdateDocumentsPattern(FolderCount);
                    break;
                case 0:
                    UpdateDocumentsPattern(FolderCount);
                    FileNameSortReversed(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);             
                    break;
            }          
        }

        public void PrintDocAndPattern(int howManyDocs,int FolderCount)
        {
            for (int i = 0; i < howManyDocs; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(session.receivedFolders[FolderCount].Documents[i].DocumentName);
                int IDInPattern = Convert.ToInt32(session.receivedFolders[FolderCount].Documents[i].InPatternID);
                --IDInPattern;


                if (IDInPattern >= 0)
                {
                    string NameInPattern = session.receivedRequiredInPatterns[IDInPattern].DocumentName;
                    textBlock.Inlines.Add(new Run("\n" + NameInPattern) { FontWeight = FontWeights.Bold, Tag = NameInPattern });
                }
                page.DocumentsListBox.Items.Add(textBlock);
            }
        }

        public void FileTimeSort(int FolderID)
        {
            var docks = session.receivedFolders[FolderID].Documents.Cast<ServerDocument>().OrderBy(d => d.DocumentID).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                session.receivedFolders[FolderID].Documents = docks;
            }
        }
        public void FileTimeSortReversed(int FolderID)
        {
            var docks = session.receivedFolders[FolderID].Documents.Cast<ServerDocument>().OrderByDescending(d => d.DocumentID).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                session.receivedFolders[FolderID].Documents = docks;
            }
        }
        public void FileNameSort(int FolderID)
        {
            var docks = session.receivedFolders[FolderID].Documents.Cast<ServerDocument>().OrderBy(d => d.DocumentName).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                session.receivedFolders[FolderID].Documents = docks;
            }
        }
        public void FileNameSortReversed(int FolderID)
        {
            var docks = session.receivedFolders[FolderID].Documents.Cast<ServerDocument>().OrderByDescending(d => d.DocumentName).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                session.receivedFolders[FolderID].Documents = docks;
            }
        }

        public void UpdateDocumentsPattern(int FolderCount)
        {

            int patternID = 0;
            if (session.receivedFolders[FolderCount].PatternID != null)
            {
                patternID = Convert.ToInt32(session.receivedFolders[FolderCount].PatternID);
            }
            else { return; }
            patternID--;

            if (patternID >= 0)
            {
                List<RequiredInPattern> NeedInFolderObj = session.receivedPatterns[patternID].RequiredInPatterns;
                List<string> NeedInFolderStr = new List<string>();
                for (int i = 0; i < NeedInFolderObj.Count; i++)
                {
                    var doc = session.receivedFolders[FolderCount].Documents.FirstOrDefault(d => d.InPatternID == NeedInFolderObj[i].DocumentPatternID);
                    if (doc == null)
                    {
                        NeedInFolderStr.Add(NeedInFolderObj[i].DocumentName);
                    }
                }

                for (int i = 0; i < NeedInFolderStr.Count; i++)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Inlines.Add(new Run("\b" + NeedInFolderStr[i]) { FontWeight = FontWeights.Bold });
                    page.DocumentsListBox.Items.Add(textBlock);
                }
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

        public void DownloadDocument(object ServerFileRoot, string FileName)
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

            Messenger.SendStrings(clientSocket,"SendPath");
            Messenger.SendStrings(clientSocket, folderPath + FileName);
            byte[] document = Messenger.ReedBytes(clientSocket);

            string[] projectName = folderPath.Split("\\");
            Directory.CreateDirectory(Settings1.Default.RootFolder + "\\" + projectName[projectName.Length - 2]);
            string PathToSave = Settings1.Default.RootFolder+"\\"+ projectName[projectName.Length-2] +"\\"+ FileName;

            using (FileStream writer = new FileStream(PathToSave, FileMode.Create))
            {
                writer.Write(document, 0, document.Length);
            }
            StyleClass.TransactionResult(Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)),page);
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
            if (Unique != "") { StyleClass.TransactionResult(Unique, page);return; }

            Messenger.SendJSON(clientSocket, departmentsIDs);
            Messenger.SendStrings(clientSocket, patternID.ToString());
            StyleClass.TransactionResult(Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)), page);
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
            if (Unique != "") { StyleClass.TransactionResult(Unique, page); return; }

            Messenger.SendJSON(clientSocket, departmentsIDs);
            Messenger.SendStrings(clientSocket, patternID.ToString());
            StyleClass.TransactionResult(Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)), page);
        }

        public void SendDocument(object ProjectName, string FileName,int nameInPattern)
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

            var folder = session.receivedFolders.FirstOrDefault(f => f.FolderPath == folderPath);
            var Doc = folder.Documents.FirstOrDefault(d => d.DocumentName ==  FileName.Split("\\").Last());
            if (Doc!=null)
            {
                if (Doc.InPatternID != null)
                { nameInPattern =Convert.ToInt32(Doc.InPatternID);}              
            }

            Messenger.SendStrings(clientSocket, "GetDocument");
            string[] FileParts = FileName.Split('\\');
            Messenger.SendStrings(clientSocket, folderPath + "\\" + FileParts[FileParts.Length-1] + "\a" + nameInPattern);
            byte[] bytes = File.ReadAllBytes(FileName);
            Messenger.SendBytes(clientSocket, bytes);

            string result = Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket));
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

        public int GetNameInPatternID(int projectIndex,string nameInPattern)
        {
            int pID = Convert.ToInt32(session.receivedFolders[projectIndex].PatternID);
            List<RequiredInPattern> requiredInPatterns = session.receivedPatterns[pID-1].RequiredInPatterns;

            RequiredInPattern filteredItems = requiredInPatterns.OfType<RequiredInPattern>()
                          .FirstOrDefault(x => x.DocumentName == nameInPattern);
            return filteredItems.DocumentPatternID;
        }

        internal Folder GetFolderByName(string folderName)
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

        public void NewOrUpdateClient(string[]clientInfo)
        {
            Messenger.SendStrings(clientSocket, "NewOrUpdateClient");
            Messenger.SendJSON(clientSocket, session.thisUser);

            Messenger.SendJSON(clientSocket, clientInfo);

            string result = Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket));
            StyleClass.TransactionResult(result, page);
        }

        public void FindClient(string ClientName)
        {
            Messenger.SendStrings(clientSocket, "FindClient");
            Messenger.SendJSON(clientSocket, session.thisUser);
            Messenger.SendStrings(clientSocket, ClientName);

            string result = Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket));
            string [] clientInfo = result.Split('\b');           

            result = Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket));
            StyleClass.TransactionResult(result, page);

            if (clientInfo.Length >= 5)
            {
                page.INNLabel.Content = clientInfo[1];
                page.EmailLabel.Content = clientInfo[2];
                page.OGRNLabel.Content = clientInfo[3];
                page.KPPLabel.Content = clientInfo[4];
            }
        }
    }
}
