using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.Entitys
{
    internal class Document
    {
        [Key]
        public long DocumentID { get; set; }
        public string Name { get; set; }
        public bool IsDone { get; set; }
        public DateTime DeadLine { get; set; }
        public bool ReadOnly { get; set; }

        public long FolderID { get; set; }
        public Folder Folder { get; set; }

        public List<Log> Logs { get; set; } = new();
    }
}
