﻿using Client.Resources.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Client.Resources.Entitys
{
    public class Folder
    {
        [Key]
        public int FolderID { get; set; }
        public string FolderPath { get; set; }

        public DateTime DeadLine { get; set; }

        public bool IsDone { get; set; }
        public int? PatternID { get; set; }
        public Pattern Pattern { get; set; }

        public int? ClientID { get; set; }
        public Customer Client { get; set; }

        public List<ServerDocument> Documents { get; set; } = new();
    }
}
