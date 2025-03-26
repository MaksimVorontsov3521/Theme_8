using System;
using Server.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.AdminFolder
{
    internal class AdminUpdateSetting
    {
        public void UpdateBaseFolder(string NewPath)
        {
            //Settings1.Default.BaseFolder = NewPath;
            //Settings1.Default.Save();


            // Получаем все подпапки
            string[] allDirectories = Directory.GetDirectories(NewPath);

            // Выводим список папок
            
            foreach (string dir in allDirectories)
            {
                Console.WriteLine("\nПапка:");
                Console.WriteLine(Path.GetFileName(dir)); // Имя папки без пути

                // Получаем все файлы (с указанием пути)
                string[] allFiles = Directory.GetFiles(NewPath+$"\\{Path.GetFileName(dir)}");

                // Выводим список файлов
                Console.WriteLine("Файлы:");
                foreach (string file in allFiles)
                {
                    Console.WriteLine(Path.GetFileName(file)); // Имя файла без пути
                }
            }
        }
    }

}
