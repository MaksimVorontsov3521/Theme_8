using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Server.Settings;
using System.IO;
using System.Xml.Linq;

namespace Server.DataBaseFolder.Querys
{
    internal class DBDocumentWork
    {

        public static int GetProjectId (string ProjectName)
        {
            ProjectName = Settings1.Default.BaseFolder +"\\"+ ProjectName+"\\";
            using (var db = new DataBase())
            {
                Folder folder = db.Folder.FirstOrDefault(f => f.FolderPath == ProjectName);
                if (folder == null)
                { return -1;}
                else { return folder.FolderID; }
            }      
        }

        public static void ChangeProject(string ProjectName, int[] departmentsIDs, int patternId,DateTime date)
        {
            ProjectName = Settings1.Default.BaseFolder + "\\" + ProjectName + "\\";
            ++patternId;
            using (var db = new DataBase())
            {

                Folder folder = db.Folder.FirstOrDefault(x => x.FolderPath == ProjectName);

                if (folder != null)
                {
                    // Изменяем значения
                    folder.PatternID = patternId;
                    folder.DeadLine = date;
                }
                db.SaveChanges();

                var entities = db.DepartmentFolder
                    .Where(x => x.FolderID == folder.FolderID)
                    .ToList();

                db.DepartmentFolder.RemoveRange(entities);
                db.SaveChanges();

                for (int i = 0; i < departmentsIDs.Length; i++)
                {
                    var DF = new DepartmentFolder
                    {
                        DepartmentID = departmentsIDs[i],
                        FolderID = folder.FolderID,
                    }
                    ;
                    db.DepartmentFolder.Add(DF);
                }
                db.SaveChanges();
            }
        }

        public static void NewProject(string ProjectName, int[] departmentsIDs, int patternId,DateTime date,string client)
        {
            Folder folder = new Folder();
            folder.FolderPath = Settings1.Default.BaseFolder + "\\" + ProjectName+"\\";
            folder.PatternID = ++patternId;
            folder.DeadLine = date;
            using (var db = new DataBase())
            {
                Client client1 = db.Client.FirstOrDefault(c => c.ClientName == client);
                folder.ClientID = client1.ClientId;

                db.Folder.Add(folder);
                db.SaveChanges();

                Folder LastId = db.Folder.FirstOrDefault(f=>f.FolderPath== folder.FolderPath);
                for (int i = 0; i < departmentsIDs.Length; i++)
                {
                    var DF = new DepartmentFolder
                    {
                        DepartmentID = departmentsIDs[i],
                        FolderID = LastId.FolderID,
                    }
                    ;
                    db.DepartmentFolder.Add(DF);
                }
                db.SaveChanges();
            }
        }

        public static int CanAddNewDocument(string path)
        {
            // 0 - Новый фалй
            // 1 - Можно перезаписать
            // 2 - Нельзя перезаписывать
            int result = 0;
            string pa = path.Split('\a').First();
            string[] st = pa.Split('\\');

            string PathToFolder = null;
            for (int i = 0; i < st.Length - 2; i++)
            {
                PathToFolder += st[i] + "\\";
            }
            using (var db = new DataBase())
            {
                var folder = db.Folder.FirstOrDefault(c => c.FolderPath == PathToFolder);
                var document = db.Document.FirstOrDefault(d => d.DocumentName == st.Last() && d.FolderID == folder.FolderID);
                if (document != null)
                {
                    result = 1;
                    if (document.DocumentReadOnly == true)
                    { result = 2; }
                }
            }
            return result;
        }

        public static void RewriteDocument(string path, string UserLogin)
        {
            string pa = path.Split('\a').First();
            string[] st = pa.Split('\\');
            string PathToFolder = null;
            for (int i = 0; i < st.Length - 2; i++)
            {
                PathToFolder += st[i] + "\\";
            }
            using (var db = new DataBase())
            {
                UserTable user = db.UserTable.FirstOrDefault(c => c.UserLogin == UserLogin);
                var folder = db.Folder.FirstOrDefault(c => c.FolderPath == PathToFolder );
                var document = db.Document.FirstOrDefault(d => d.DocumentName == st.Last() && d.FolderID == folder.FolderID);

                if (folder != null && document!=null)
                {
                    string nameInPattern = path.Split("\a").Last();
                    int? inPatternID = Convert.ToInt32(nameInPattern);
                    if (inPatternID != -1)
                    {
                        var notUniqueDoc = db.Document.FirstOrDefault(d => d.InPatternID== inPatternID && d.FolderID == folder.FolderID);
                        if (notUniqueDoc != null)
                        {
                            notUniqueDoc.InPatternID = null;
                            var Log1 = new LogTable
                            {
                                LogAction = 1,
                                DocumentID = notUniqueDoc.DocumentID,
                                UserID = user.UserID,
                            };
                            db.LogTable.Add(Log1);
                            db.SaveChanges();
                        }
                        document.InPatternID = inPatternID;
                    }
                    else
                    {
                        document.InPatternID=null;
                    }
                    
                    var Log = new LogTable
                    {
                        LogAction = 3,
                        DocumentID = document.DocumentID,
                        UserID = user.UserID
                    };
                    db.Document.Update(document);
                    db.LogTable.Add(Log);
                    db.SaveChanges();
                }
            }
        }

        public static bool IsAClient(string client)
        {
            using (var db = new DataBase())
            {
                Client client1 = db.Client.FirstOrDefault(c => c.ClientName == client);
                if (client1 != null)
                { return true; }
                else { return false; }
            }
        }

        public static void AddNewDocument(string path,string UserLogin)
        {
            string pa = path.Split('\a').First();
            string[] st = pa.Split('\\');
            string PathToFolder=null;
            for (int i = 0; i < st.Length - 2; i++)
            {
                PathToFolder += st[i]+"\\";
            }
            using (var db = new DataBase())
            {
                string nameInPattern = path.Split("\a").Last();
                int? inPatternID = Convert.ToInt32(nameInPattern);
                

                UserTable user = db.UserTable.FirstOrDefault(c => c.UserLogin == UserLogin);
                var folder = db.Folder.FirstOrDefault(c => c.FolderPath == PathToFolder);

                if (folder != null)
                {
                    var NewDoc = new Entitys.Document
                    {
                        DocumentName = st[st.Length - 1],
                        FolderID = folder.FolderID,                        
                    };

                    if (inPatternID != -1)
                    {
                        var notUniqueDoc = db.Document.FirstOrDefault(d => d.InPatternID == inPatternID && d.FolderID == folder.FolderID);
                        if (notUniqueDoc != null)
                        {
                            notUniqueDoc.InPatternID = null;
                            var Log1 = new LogTable
                            {
                                LogAction = 1,
                                DocumentID = notUniqueDoc.DocumentID,
                                UserID = user.UserID,
                            };
                            db.LogTable.Add(Log1);
                            db.SaveChanges();

                        }
                        NewDoc.InPatternID = inPatternID;
                    }
                    else
                    {
                        NewDoc.InPatternID = null;
                    }

                    db.Document.Add(NewDoc);
                    db.SaveChanges();

                    int? lastId = db.Document.Max(p => (int?)p.DocumentID);

                    var Log = new LogTable
                    {
                        LogAction = 1,
                        DocumentID = lastId,
                        UserID = user.UserID,
                    };
                    db.LogTable.Add(Log);
                    db.SaveChanges();
                }
            }
        }
        public static void UpdateFolder(Folder folder)
        {
            using (var db = new DataBase())
            {
                db.Folder.Update(folder);
                db.SaveChanges();
            }
        }
    }
}
