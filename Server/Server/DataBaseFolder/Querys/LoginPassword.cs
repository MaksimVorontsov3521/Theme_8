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

        public static UserTable Login(string login, string enteredPassword)
        {
            using (var context = new DataBase())
            {
                var X = context.UserTable.FirstOrDefault(w => w.UserLogin == login);

                if(X==null)
                { return null; }

                if (Security.Security.VerifyPassword(enteredPassword, X.UserPassword) == true)
                { 
                return X;
                }

                return null;
            }
        }

        public static int GetLevel(int UserRoleID)
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
                var X = context.UserTable.FirstOrDefault(w => w.UserLogin == login && w.RoleID == 1);

                if (X == null)
                { return false; }

                if (Security.Security.VerifyPassword(enteredPassword, X.UserPassword) == true)
                {
                    return true;
                }

                return false;
            }
        }

    }
}
