using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.Entitys
{
    internal class User
    {
        [Key]
        public long UserID { get; set; }
        public string NameUser { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }
        public string LoginUser { get; set; }
        public string PasswordUser { get; set; }

        public long RoleID { get; set; }
        public Role RoleTable { get; set; }

        public long DepartmentID { get; set; }
        public Department Department { get; set; }

        public List<Log> Logs { get; set; } = new();
    }
}
