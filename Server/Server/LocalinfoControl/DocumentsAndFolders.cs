using Server.DataBaseFolder.Querys;
using Server.Settings;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LocalinfoControl
{
    internal class DocumentsAndFolders
    {
        public static byte[] ToSendPath(string Path)
        {
            try
            {
                // Чтение всех байтов из файла
                byte[] bytes = File.ReadAllBytes(Path);
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return null;
            }
        }

        public static void GetDocument(string Path, byte[] document)
        {
            using (FileStream writer = new FileStream(Path, FileMode.Create))
            {
                writer.Write(document, 0, document.Length);
            }
        }

        public static void isTimeForBackup()
        {
            string backupFolder = Settings1.Default.BackupFolder;
            var newestByCreationTime = Directory
        .GetFiles(backupFolder)
        .Select(f => new FileInfo(f))
        .OrderByDescending(f => f.CreationTime)
        .First();

            if ((DateTime.Now - newestByCreationTime.CreationTime).TotalDays > Settings2.Default.BackupSchedule)
            {
                // Вычисляем время до полуночи
                DateTime now = DateTime.Now;
                DateTime midnight = now.Date.AddDays(1);
                TimeSpan timeUntilMidnight = midnight - now;

                System.Timers.Timer timer = new System.Timers.Timer
                {
                    Interval = timeUntilMidnight.TotalMilliseconds,
                    AutoReset = false
                };

                timer.Elapsed += (sender, e) =>
                {
                    Backup();
                };
                timer.Start();
            }

        }

        public static void Backup()
        {
            string sourceFolder = Settings1.Default.BaseFolder;
            string backupFolder = Settings1.Default.BackupFolder;
            string zipFileName = $"Backup_{DateTime.Now:yyyyMMdd}.zip"; // Имя архива

            string zipPath = backupFolder + "\\" + zipFileName;

            try
            {
                if (!Directory.Exists(backupFolder))
                    Directory.CreateDirectory(backupFolder);

                if (File.Exists(zipPath))
                { return; }

                ZipFile.CreateFromDirectory(sourceFolder, zipPath, CompressionLevel.Optimal, false);
                Console.WriteLine($"Бэкап создан: {zipPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            if (Settings2.Default.KeepBackups > 0)
            {
                var oldBackups = Directory.GetFiles(backupFolder, "*.zip")
                .Where(f => (DateTime.Now - File.GetCreationTime(f)).TotalDays > Settings2.Default.BackupSchedule * (Settings2.Default.KeepBackups + 1));

                foreach (var oldBackup in oldBackups)
                {
                    File.Delete(oldBackup);
                    Console.WriteLine($"Удалён старый бэкап: {oldBackup}");
                }
            }
        }

    }
}

