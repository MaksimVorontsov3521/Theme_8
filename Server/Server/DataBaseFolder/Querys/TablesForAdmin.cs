using Server.DataBaseFolder.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.AdminFolder;
using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.Entitys;

namespace Server.DataBaseFolder.Querys
{
    internal class TablesForAdmin
    {
        private readonly DataBase dbContext;

        public TablesForAdmin(DataBase dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<User> UserTable()
        {
            List<User> users = new List<User>();
            using (var context = new DataBase())
            {
                var X = context.UserTable;

                foreach (var worker in X)
                {
                    users.Add(worker);
                }
                return users;
            }
        }

    }
}
