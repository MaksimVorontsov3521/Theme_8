using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Resources
{
    internal class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int DepartmentId { get; set; }

        public List<Log> Logs { get; set; } = new();
    }
}
