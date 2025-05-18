using Server.DataBaseFolder;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.AdminFolder
{
    internal class AdminAdd
    {
        internal void AddDepartment(string departmentName)
        {
            using (var context = new DataBase())
            {
                Department Department = new Department { DepartmentName = departmentName };
                context.Department.Add(Department);
                context.SaveChanges();
            }
        }

        internal void UpdateClient(byte[] userByte)
        {
            UserTable user = JsonSerializer.Deserialize<UserTable>(userByte);
            using (var context = new DataBase())
            {
                context.UserTable.Update(user);

                context.SaveChanges();
            }
        }


        internal void AddClient(byte[] userByte)
        {
            UserTable user = JsonSerializer.Deserialize<UserTable>(userByte);
            using (var context = new DataBase())
            {
                context.UserTable.Add(user);

                context.SaveChanges();
            }
        }
    }
}
