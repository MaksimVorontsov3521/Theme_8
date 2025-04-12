using Server.DataBaseFolder;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


    }
}
