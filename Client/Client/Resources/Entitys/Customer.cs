﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Resources.Entitys
{
    public class Customer
    {
        [Key]
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string? INN { get; set; }
        public string? Email { get; set; }
        public string? OGRN { get; set; }
        public string? KPP { get; set; }
    }
}
