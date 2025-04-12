using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class DepartmentFolder
    {
        [Key]
        public int DepartmentFolderID { get; set; }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        public int FolderID { get; set; }
        public Folder Folder { get; set; }

    }
}
