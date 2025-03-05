using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.Entitys
{
    internal class Folder
    {
        [Key]
        public long FolderID { get; set; }
        public string FolderPath { get; set; }

        public long? PatternID { get; set; }
        public Pattern Pattern { get; set; }

        public long ClientID { get; set; }
        public Client Client { get; set; }

        public List<Document> Documents { get; set; } = new();
        public List<Department> Departments { get; set; } = new();
    }
}
