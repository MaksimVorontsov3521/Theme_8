using System;
using Server.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.Entitys;
using Server.DataBaseFolder;

namespace Server.AdminFolder
{
    internal class AdminUpdateSetting
    {
        public void UpdateBaseFolder(string NewPath)
        {
            Settings1.Default.BaseFolder = NewPath;
            Settings1.Default.Save();
            // Получаем все подпапки
            string[] allDirectories = Directory.GetDirectories(NewPath);

            // Выводим список папок

            using (DataBase context = new DataBase())
            {
                foreach (string dir in allDirectories)
                {
                    string FolderPath = NewPath + "\\" + Path.GetFileName(dir)+"\\";

                    // Находим первый товар с таким именем (или null, если не найден)
                    Folder IsFolder = context.Folder
                        .FirstOrDefault(p => p.FolderPath == FolderPath);

                    if (IsFolder != null)
                    {
                        continue;
                    }

                    Folder folder = new Folder {FolderPath= $"{NewPath + "\\" + Path.GetFileName(dir)+"\\"}"};
                    context.Folder.Add(folder);
                    context.SaveChanges();
                    int lastFolderID = context.Folder.Max(p => p.FolderID);

                    // Получаем все файлы (с указанием пути)
                    string[] allFiles = Directory.GetFiles(NewPath + $"\\{Path.GetFileName(dir)}");
                    string[] docName = new string[allFiles.Length];
                    for (int i = 0; i < allFiles.Length; i++)
                    {
                        docName[i]=allFiles[i].Split("\\").Last();
                    }                   

                    List<Document> documents = new List<Document>();
                    
                    for (int i=0;i<docName.Length;i++)
                    {                        
                        documents.Add(new Document { FolderID = lastFolderID, DocumentName = docName[i],DocumentReadOnly=false });
                    }
                    context.AddRange(documents);
                    context.SaveChanges();
                }
            }
        }
    }

}
