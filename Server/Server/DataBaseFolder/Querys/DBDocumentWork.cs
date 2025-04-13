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

namespace Server.DataBaseFolder.Querys
{
    internal class DBDocumentWork
    {

        public int GetProjectId (string ProjectName)
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

        public void ChangeProject(string ProjectName, int[] departmentsIDs, int patternId)
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

        public void NewProject(string ProjectName, int[] departmentsIDs, int patternId)
        {
            Folder folder = new Folder();
            folder.FolderPath = Settings1.Default.BaseFolder + "\\" + ProjectName+"\\";
            folder.PatternID = ++patternId;
            using (var db = new DataBase())
            {
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

        public int CanAddNewDocument(string path)
        {
            // 0 - Новый фалй
            // 1 - Можно перезаписать
            // 2 - Нельзя перезаписывать
            int result = 0;
            string[] st = path.Split('\\');

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

        public void RewriteDocument(string path, string UserLogin)
        {
            string[] st = path.Split('\\');
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
                    var Log = new LogTable
                    {
                        LogAction = 3,
                        DocumentID = document.DocumentId,
                        UserID = user.UserID
                    };
                    db.LogTable.Add(Log);
                    db.SaveChanges();
                }
            }
        }
        public void AddNewDocument(string path,string UserLogin)
        {
            string[] st = path.Split('\\');
            string PathToFolder=null;
            for (int i = 0; i < st.Length - 2; i++)
            {
                PathToFolder += st[i]+"\\";
            }
            using (var db = new DataBase())
            {
                UserTable user = db.UserTable.FirstOrDefault(c => c.UserLogin == UserLogin);
                var folder = db.Folder.FirstOrDefault(c => c.FolderPath == PathToFolder);

                if (folder != null)
                {
                    var NewDoc = new Entitys.Document
                    {
                        DocumentName = st[st.Length - 1],
                        FolderID = folder.FolderID,
                    };

                    db.Document.Add(NewDoc);
                    db.SaveChanges();

                    int? lastId = db.Document.Max(p => (int?)p.DocumentId);

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
    }
}
