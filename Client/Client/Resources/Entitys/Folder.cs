﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Entitys
{
    internal class Folder
    {
        [Key]
        public int FolderID { get; set; }
        public string FolderPath { get; set; }

        public int? PatternID { get; set; }
        public Pattern Pattern { get; set; }

        public List<ServerDocument> Documents { get; set; } = new();
    }
}
