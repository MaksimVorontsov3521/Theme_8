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

        public UserTable Login(string login, string enteredPassword)
        {
            using (var context = new DataBase())
            {
                var X = context.UserTable
                    .Where(w => w.UserLogin == login && w.UserPassword == enteredPassword);

                foreach (var worker in X)
                {
                    return worker;                 
                }
                return null;
            }
        }

        public int GetLevel(int UserRoleID)
        {
            using (var context = new DataBase())
            {
                var X = context.RoleTable
                    .Where(w => w.RoleId == UserRoleID);

                foreach (var role in X)
                {
                    return role.RoleLevel;
                }
                return 100;
            }
        }

        public bool LoginAdmin(string login, string enteredPassword)
        {
            using (var context = new DataBase())
            {
                var X = context.UserTable
                    .Where(w => w.UserLogin == login && w.UserPassword == enteredPassword && w.RoleID == 1)
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
