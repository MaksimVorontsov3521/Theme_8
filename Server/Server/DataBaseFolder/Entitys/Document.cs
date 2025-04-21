using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class Document
    {
        [Key]
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public bool IsDone { get; set; }
        public DateTime DeadLine { get; set; }
        public bool DocumentReadOnly { get; set; }
        public int? InPatternID { get; set; }

        public int FolderID { get; set; }

        public List<LogTable> Logs { get; set; } = new();
    }
}
