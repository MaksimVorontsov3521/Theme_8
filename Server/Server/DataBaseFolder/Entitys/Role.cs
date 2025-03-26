using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class RoleTable
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string? Rights { get; set; }
        public int RoleLevel { get; set; }

        public List<UserTable> Users { get; set; } = new();
    }
}
