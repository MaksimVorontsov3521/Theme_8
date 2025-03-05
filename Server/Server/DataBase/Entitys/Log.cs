using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.Entitys
{
    internal class Log
    {
        [Key]
        public long LogID { get; set; }
        public long UserID { get; set; }
        public User User { get; set; }

        public long? DocumentID { get; set; }
        public Document Document { get; set; }

        public long LogActionID { get; set; }
        public LogAction Action { get; set; }
    }
}
