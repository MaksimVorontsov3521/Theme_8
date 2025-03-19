using Server.DataBaseFolder.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Admin
{ 
    public class Admin
    {
        private int port = 9091;
        private TcpListener adminServer;
        async public void Connection()
        {       
            adminServer = new TcpListener(port);
            adminServer.Start();
            Console.WriteLine("Порт для админа запущен");
        }
    }
}
