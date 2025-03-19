using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Session
{
    internal class UserSession
    {
        public static int userId;
        public static string userName;
        public static int roleID;
        public static int departmentID;
        public static int level;
        public List<UserSessionFolders> userSessionFolders;
    }
    internal class UserSessionFolders
    {
        public static int folderId;
        public static string folderPath;
        public List<Document> docsInPattern;
        public List<string> docsInFolder;
    }
    internal class Document
    {
        public int documentID;
        public string documentName;
        public bool isDone;
        public DateTime deadLine;
        public bool readOnly;
        public string nameInPattertn;
    }
}