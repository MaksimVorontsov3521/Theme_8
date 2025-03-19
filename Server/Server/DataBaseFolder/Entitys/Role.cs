using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string? Rights { get; set; }
        public int Level { get; set; }

        public List<User> Users { get; set; } = new();
    }
}
