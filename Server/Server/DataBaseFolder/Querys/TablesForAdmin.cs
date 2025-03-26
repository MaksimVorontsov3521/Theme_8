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

        public List<UserTable> UserTable()
        {
            List<UserTable> users = new List<UserTable>();
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

        public List<RoleTable> RoleTable()
        {
            List<RoleTable> users = new List<RoleTable>();
            using (var context = new DataBase())
            {
                var X = context.RoleTable;

                foreach (var worker in X)
                {
                    users.Add(worker);
                }
                return users;
            }
        }

        public List<LogTable> LogTable()
        {
            List<LogTable> users = new List<LogTable>();
            using (var context = new DataBase())
            {
                var X = context.Logs;

                foreach (var worker in X)
                {
                    users.Add(worker);
                }
                return users;
            }
        }

        public List<LogAction> LogActionTable()
        {
            List<LogAction> users = new List<LogAction>();
            using (var context = new DataBase())
            {
                var X = context.LogAction;

                foreach (var worker in X)
                {
                    users.Add(worker);
                }
                return users;
            }
        }

    }
}
