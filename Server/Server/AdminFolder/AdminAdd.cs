using Server.DataBaseFolder;
using Server.DataBaseFolder.Entitys;
using Server.DataBaseFolder.Querys;
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

        internal string AddRole(byte[] bytes)
        {
            RoleTable newRole = JsonSerializer.Deserialize<RoleTable>(bytes);
            using (var context = new DataBase())
            {              
                RoleTable role = context.RoleTable.FirstOrDefault(r=>r.RoleId == newRole.RoleId);
                if (role == null)
                {
                    RoleTable SameName = context.RoleTable.FirstOrDefault(r => r.RoleName == newRole.RoleName);
                    if (SameName != null)
                    { return "Такая роль уже существует"; }

                    context.RoleTable.Add(newRole);
                    context.SaveChanges();
                }
                else
                {
                    RoleTable SameName = context.RoleTable.FirstOrDefault(r => r.RoleName == newRole.RoleName &&
                    r.RoleId!=newRole.RoleId);
                    if (SameName != null)
                    { return "Такая роль уже существует"; }

                    role.RoleLevel = newRole.RoleLevel;
                    role.RoleName = newRole.RoleName;
                    role.Rights = newRole.Rights;
                    context.RoleTable.Update(role);
                    context.SaveChanges();
                }
                return "Успешно";
            }
        }

        internal string AddDepartment(string departmentName)
        {
            using (var context = new DataBase())
            {
                Department departmentSameName = context.Department.FirstOrDefault(d => d.DepartmentName == departmentName);
                if (departmentSameName != null)
                {
                    return "Такой отдел уже существует";
                }

                Department Department = new Department { DepartmentName = departmentName };
                context.Department.Add(Department);
                context.SaveChanges();
                return "Успешно";
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
