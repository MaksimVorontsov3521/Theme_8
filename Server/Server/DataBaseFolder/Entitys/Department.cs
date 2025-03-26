using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public List<UserTable> Users { get; set; } = new();
        public List<Folder> Folders { get; set; } = new();
    }
}
