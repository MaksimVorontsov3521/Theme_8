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
        public int Id { get; set; }
        public string Name { get; set; }

        public int PatternId { get; set; }
        public Pattern Pattern { get; set; }
    }
}
