using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Settings;

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
                var X = context.UserTable.FirstOrDefault(w => w.UserLogin == login && w.RoleID != 1);

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
                bool[] Rights = new bool[2];
                RoleTable user = context.RoleTable
                    .FirstOrDefault(w => w.RoleId == UserRoleID);
                if (user.RoleId != null)
                { return user.RoleLevel; }else { return 100; }
                
            }
        }

        public static bool[] CnaLevel(int userLevel)
        {
            bool[] Rights = new bool[2];
            if (userLevel > Settings2.Default.CanCreateNewProject)
            { Rights[0] = false; }
            else { Rights[0] = true; }

            if (userLevel > Settings2.Default.CanEditClient)
            { Rights[1] = false; }
            else { Rights[1] = true; }
            return Rights;
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
