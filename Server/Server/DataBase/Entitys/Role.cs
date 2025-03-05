using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.Entitys
{
    internal class Role
    {
        [Key]
        public long RoleID { get; set; }
        public string RoleName { get; set; }
        public string Rights { get; set; }
        public long Level { get; set; }
        public List<User> Users { get; set; } = new();
    }
}
