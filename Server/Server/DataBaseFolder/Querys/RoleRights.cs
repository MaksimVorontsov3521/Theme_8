using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.Entitys;
using Server.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Querys
{
    internal class RoleRights
    {
        private readonly DataBase dbContext;

        public RoleRights(DataBase dbContext)
        {
            this.dbContext = dbContext;
        }
        static public RoleTable GetUserRole(string login, string enteredPassword)
        {
            using (var context = new DataBase())
            {
                var Worker = context.UserTable
                    .FirstOrDefault(w => w.UserLogin == login && w.UserPassword == enteredPassword); // два условия
                if (Worker != null)
                { RoleTable roleTable = new RoleTable();
                    return roleTable;
                }
                var Role = context.RoleTable.FirstOrDefault(r => r.RoleId == Worker.RoleID);
                return Role;
            }
        }
        static public bool CanEditClient(string login, string enteredPassword)
        {
            RoleTable roleTable = GetUserRole(login, enteredPassword);
            if (roleTable == null) { return false; }
            if (roleTable.RoleLevel > Settings2.Default.CanEditClient)
            {return false; }
            else
            { return true; }
        }

    }
}
