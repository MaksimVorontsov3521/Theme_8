using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Querys
{
    internal class ClientTables
    {
        private readonly DataBase dbContext;
        public ClientTables(DataBase dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Folder> FoldersForClient()
        {
            List<Folder> Folders = new List<Folder>();
            using (var context = new DataBase())
            {
                var X = context.Folder;

                foreach (var folder in X)
                {
                    Folders.Add(folder);
                }
                return Folders;
            }
        }

        public List<Document> DocumentsForClient()
        {
            List<Document> Documents = new List<Document>();
            using (var context = new DataBase())
            {
                var X = context.Document;

                foreach (var document in X)
                {
                    Documents.Add(document);
                }
                return Documents;
            }
        }


        public List<Pattern> PatternsForClient()
        {
            List<Pattern> Documents = new List<Pattern>();
            using (var context = new DataBase())
            {
                var X = context.Pattern;

                foreach (var document in X)
                {
                    Documents.Add(document);
                }
                return Documents;
            }
        }

        public List<RequiredInPattern> RequiredInPatternsForClient()
        {
            List<RequiredInPattern> Documents = new List<RequiredInPattern>();
            using (var context = new DataBase())
            {
                var X = context.RequiredInPattern;

                foreach (var document in X)
                {
                    Documents.Add(document);
                }
                return Documents;
            }
        }


    }
}
