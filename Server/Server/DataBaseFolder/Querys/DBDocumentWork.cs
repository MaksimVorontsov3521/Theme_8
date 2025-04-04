using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Querys
{
    internal class DBDocumentWork
    {

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
                var user = db.UserTable.First(c => c.UserName == UserLogin);
                var folder = db.Folder.FirstOrDefault(c => c.FolderPath == PathToFolder );
                var document = db.Document.FirstOrDefault(d => d.DocumentName == st.Last() && d.FolderID == folder.FolderID);

                if (folder != null && document!=null)
                {
                    var Log = new Entitys.LogTable
                    {
                        LogAction = 3,
                        DocumentID = document.DocumentId,
                        UserID = user.UserId
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
                        UserID = user.UserId,
                    };
                    db.LogTable.Add(Log);
                    db.SaveChanges();
                }
            }
        }
    }
}
