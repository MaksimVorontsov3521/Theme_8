using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.Entitys
{
    internal class LogAction
    {
        [Key]
        public long ActionID { get; set; }
        public string ActionName { get; set; }
        public List<Log> Logs { get; set; } = new();
    }
}
