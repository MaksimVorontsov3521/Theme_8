using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class Department_Folder
    {
        [Key]
        public int DepartmentFolderId { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int FolderId { get; set; }
        public Folder Folder { get; set; }

    }
}
