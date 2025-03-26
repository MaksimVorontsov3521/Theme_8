using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.DbContexts
{

    internal class LoginPassword
    {
        private readonly DataBase dbContext;

        public LoginPassword(DataBase dbContext)
        {
            this.dbContext = dbContext;
        }

        public int Login(string login, string enteredPassword)
        {
            using (var context = new DataBase())
            {
                var X = context.UserTable
                    .Include(w => w.RoleId) 
                    .Where(w => w.UserLogin == login && w.UserPassword == enteredPassword) // два условия
                    .ToList();

                foreach (var worker in X)
                {
                    return worker.RoleId;
                }
                return -1;

            }
        }

        public bool LoginAdmin(string login, string enteredPassword)
        {
            using (var context = new DataBase())
            {
                var X = context.UserTable
                    .Where(w => w.UserLogin == login && w.UserPassword == enteredPassword && w.RoleId == 1) // три условия
                    .ToList();

                foreach (var worker in X)
                {
                    return true;
                }
                return false;

            }
        }

    }
}
