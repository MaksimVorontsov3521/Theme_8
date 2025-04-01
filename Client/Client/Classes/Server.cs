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

namespace Client
{
    public class Server
    {
        MainWindow mainWindow;
        ReadAndWrite Messenger = new ReadAndWrite();

        Socket clientSocket;
        string serverIP = Settings1.Default.ServerURL;
        int port = Settings1.Default.ServerPort;

        internal List<ServerDocument> receivedDocuments = new List<ServerDocument>();
        internal List<Folder> receivedFolders = new List<Folder>();
        internal List<Pattern> receivedPatterns = new List<Pattern>();
        internal List<RequiredInPattern> receivedRequiredInPatterns = new List<RequiredInPattern>();

        public Server(MainWindow mainWindow)
        {
            // Связка окна и сервера
            this.mainWindow = mainWindow;
            Server server = this;
            Entrance entrance = new Entrance(server);
            mainWindow.WorkPlace.Navigate(entrance);
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
           MessageBox.Show(Encoding.UTF8.GetString(Messenger.ReedBytes(clientSocket)));
            
            GetTables();
            CreateWorkPlace();
        }

        public void CreateWorkPlace()
        {
            MainWorkPage page = new MainWorkPage(this);
            for (int i = 0; i < receivedFolders.Count; i++)
            {
                page.ProjectsListBox.Items.Add(receivedFolders[i].FolderPath);
            }
            mainWindow.MainWorkPageNavigate(page);
        }

        public void UpdateDocuments(MainWorkPage page,int FolderCount)
        {
            int folderID = receivedFolders[FolderCount].FolderID;


            page.DocumentsListBox.Items.Clear();
            for (int i = 0; i < receivedDocuments.Count; i++)
            {
                if (receivedDocuments[i].FolderId == folderID)
                {
                    page.DocumentsListBox.Items.Add(receivedDocuments[i].DocumentName);
                }
            }
        }

        public void GetTables()
        {
            byte[] jsonBytes = Messenger.ReedBytes(clientSocket);
            string json = Encoding.UTF8.GetString(jsonBytes);
            receivedDocuments = JsonSerializer.Deserialize<List<ServerDocument>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedFolders = JsonSerializer.Deserialize<List<Folder>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedPatterns = JsonSerializer.Deserialize<List<Pattern>>(json);

            jsonBytes = Messenger.ReedBytes(clientSocket);
            json = Encoding.UTF8.GetString(jsonBytes);
            receivedRequiredInPatterns = JsonSerializer.Deserialize<List<RequiredInPattern>>(json);
        }
    }
}
