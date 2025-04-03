﻿using Server.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.DataBaseFolder;
using System.Net.Sockets;

namespace Server.Session
{
    internal class UserSession
    {
        internal ReadAndWrite Messenger { get; set; }
        internal DataBase dataBase { get; set; }
        internal Socket clientSocket { get; set; }
        internal string userLogin { get; set; }
    }
}