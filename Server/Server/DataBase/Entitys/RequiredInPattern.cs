using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.Entitys
{
    internal class RequiredInPattern
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }

        public long PatternID { get; set; }
        public Pattern Pattern { get; set; }
    }
}
