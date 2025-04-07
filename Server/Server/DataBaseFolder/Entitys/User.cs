using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class UserTable
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }

        public int RoleID { get; set; }
        public RoleTable Role { get; set; }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        public List<LogTable> Logs { get; set; } = new();
    }
}
