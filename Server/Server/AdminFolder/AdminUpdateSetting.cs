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
                Console.WriteLine("Обновление корневой папки");
                foreach (string dir in allDirectories)
                {
                    Console.WriteLine("\nПапка:");
                    Console.WriteLine(NewPath + "\\" + Path.GetFileName(dir)); // Имя папки без пути

                    string FolderPath = NewPath + "\\" + Path.GetFileName(dir);

                    // Находим первый товар с таким именем (или null, если не найден)
                    Folder IsFolder = context.Folder
                        .FirstOrDefault(p => p.FolderPath == FolderPath);

                    if (IsFolder != null)
                    {
                        Console.WriteLine($"Папка с таким путём уже существует");
                        continue;
                    }

                    Folder folder = new Folder {FolderPath= $"{NewPath + "\\" + Path.GetFileName(dir)}"};
                    context.Folder.Add(folder);
                    context.SaveChanges();

                    // Получаем все файлы (с указанием пути)
                    string[] allFiles = Directory.GetFiles(NewPath + $"\\{Path.GetFileName(dir)}");

                    int FolderId = folder.FolderID;

                    List<Document> documents = new List<Document>();
                    // Выводим список файлов
                    Console.WriteLine("Файлы:");
                    foreach (string file in allFiles)
                    {
                        Console.WriteLine(Path.GetFileNameWithoutExtension(file));
                        string StringDocumentName = Path.GetFileNameWithoutExtension(file);
                        // Имя файла без пути
                        documents.Add(new Document { FolderId = FolderId, DocumentName = StringDocumentName, IsDone = true,DocumentReadOnly=false });
                    }
                    context.AddRange(documents);
                    context.SaveChanges();
                }
            }
        }
    }

}
