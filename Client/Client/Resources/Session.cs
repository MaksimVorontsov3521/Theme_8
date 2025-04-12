using Client.Resources.Entitys;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Resources
{
    internal class Session
    {
        internal ThisUser thisUser { get; set; }
        internal int level { get; set; }
        internal List<ServerDocument> receivedDocuments = new List<ServerDocument>();
        internal List<Folder> receivedFolders = new List<Folder>();
        internal List<Pattern> receivedPatterns = new List<Pattern>();
        internal List<RequiredInPattern> receivedRequiredInPatterns = new List<RequiredInPattern>();
        internal List<Department> department = new List<Department>();
    }
}
