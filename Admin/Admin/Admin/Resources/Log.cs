using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Resources
{
    internal class Log
    {
        [Key]
        public int LogId { get; set; }
        public int LogActionId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? DocumentId { get; set; }
        public Document? Document { get; set; }

        public LogAction LogAction { get; set; }
    }
}
