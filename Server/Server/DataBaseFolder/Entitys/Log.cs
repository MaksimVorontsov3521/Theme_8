using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class LogTable
    {
        [Key]
        public int LogID { get; set; }
        public int UserID { get; set; }
        public UserTable User { get; set; }

        public int? DocumentID { get; set; }
        public Document? Document { get; set; }
        public int LogAction { get; set; }
        //public LogAction LogActionId { get; set; }
    }
}
