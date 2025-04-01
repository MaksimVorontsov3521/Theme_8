using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class Pattern
    {
        [Key]
        public int PatternId { get; set; }
        public string PatternName { get; set; }
        public string? Description { get; set; }

        public List<RequiredInPattern> RequiredInPatterns { get; set; } = new();
        public List<Folder> Folders { get; set; } = new();
    }
}
