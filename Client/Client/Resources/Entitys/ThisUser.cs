using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Resources.Entitys
{
    class ThisUser
    {

        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }
    }
}
