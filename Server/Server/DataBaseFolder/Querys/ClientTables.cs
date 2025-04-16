using Microsoft.EntityFrameworkCore;
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

        public List<Folder> FoldersForClient(int departmentId)
        {
            
            using (var context = new DataBase())
            {
                var availableFolders = context.DepartmentFolder
                    .Where(fd => fd.DepartmentID == departmentId)
                    .Select(fd => fd.Folder)
                    .Distinct()
                    .Include(f => f.Documents) // Загружаем документы сразу
                    .ToList();

                return availableFolders;
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


        public List<Department> DepartmentsForClient()
        {
            List<Department> Documents = new List<Department>();
            using (var context = new DataBase())
            {
                var X = context.Department;

                foreach (var document in X)
                {
                    Documents.Add(document);
                }
                return Documents;
            }
        }
    }
}
