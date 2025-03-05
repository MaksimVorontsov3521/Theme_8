using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.Entitys
{
    internal class Department_Folder
    {
        [Key]
        public long Department_Folder_ID { get; set; }
        public long DepartmentID { get; set; }
        public Department Department { get; set; }
        public long FolderID { get; set; }
        public Folder Folder { get; set; }

    }
}
