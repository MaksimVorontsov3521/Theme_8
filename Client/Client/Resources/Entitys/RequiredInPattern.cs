using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class RequiredInPattern
    {
        [Key]
        public int DocumentPatternID { get; set; }
        public string DocumentName { get; set; }

        public int PatternID { get; set; }
        public Pattern Pattern { get; set; }
    }
}
