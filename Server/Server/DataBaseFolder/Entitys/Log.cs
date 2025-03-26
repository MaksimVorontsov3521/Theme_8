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
        public int LogId { get; set; }
        public int LogActionId { get; set; }

        public int UserId { get; set; }
        public UserTable User { get; set; }

        public int? DocumentId { get; set; }
        public Document? Document { get; set; }

        public LogAction LogAction { get; set; }
    }
}
