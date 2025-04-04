using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class LogAction
    {
        [Key]
        public int ActionID { get; set; }
        public string ActionName { get; set; }

        //public List<LogTable> Logs { get; set; } = new();
    }
}
