using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LocalinfoControl
{
    internal class DocumentsAndFolders
    {
        public byte[] ToSendPath(string Path)       
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

    }
}
