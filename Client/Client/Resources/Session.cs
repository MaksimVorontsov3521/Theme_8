using Client.Resources.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Resources
{
    public class Session
    {
        public ThisUser thisUser { get; set; }
        public List<Folder> receivedFolders = new List<Folder>();
        public List<Pattern> receivedPatterns = new List<Pattern>();
        public List<RequiredInPattern> receivedRequiredInPatterns = new List<RequiredInPattern>();
        public List<Department> department = new List<Department>();
    }
}
