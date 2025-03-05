using Microsoft.EntityFrameworkCore;
using Server.DataBase.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase.DbContexts
{

    internal class LoginPassword
    {
        private readonly DataBase dbContext;

        public LoginPassword(DataBase dbContext)
        {
            this.dbContext = dbContext;
        }

        public long Login(string login, string enteredPassword)
        {
            using (var context = new DataBase())
            {
                var X = context.UserTable
                    .Include(w => w.RoleTable) 
                    .Where(w => w.LoginUser == login && w.PasswordUser == enteredPassword) // два условия
                    .ToList();

                foreach (var worker in X)
                {
                    return worker.RoleID;
                }
                return -1;

                //foreach (var worker in workersWithRoles)
                //{
                //    return true;
                //}
            }
        }

    }
}
