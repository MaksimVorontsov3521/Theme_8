using Admin.Pages;
using Admin.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static Admin.Classes.StyleClass;

namespace Admin.Classes
{
    internal class StyleClass
    {
        public static void TransactionResult(string Result, MainWorkPage page)
        {
            //
            if (Result.Contains("Успешно"))
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightPink);
            }

            // Таймер для запуска анимации через 2 секунды
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (sender, e) =>
            {
                page.Popup1.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        public static void TransactionResult(string Result, MainWindow page)
        {
            //
            if (Result.Contains("Успешно"))
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightPink);
            }

            // Таймер для запуска анимации через 2 секунды
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (sender, e) =>
            {
                page.Popup1.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        public class ShowUser
        { 
            public int ID { get; set; }
            public string Имя { get; set; }
            public string Фамилия { get; set; }
            public string Отчество { get; set; }
            public string Телефон { get; set; }
            public string Логин { get; set; }
            public string Почта { get; set; }
            public string Роль { get; set; }
            public string Отдел { get; set; }
            public bool Заблокирован { get; set; }

        }

        public static void PrintRoleGrid(DataGrid grid, Server server)
        {
            ObservableCollection<Role> showRole = new ObservableCollection<Role>();

            List<Role> role = server.receivedRole;

            for (int i = 0; i < role.Count; i++)
            {
                showRole.Add(new Role
                {
                    RoleId = role[i].RoleId, RoleName =role[i].RoleName, Rights = role[i].Rights, RoleLevel = role[i].RoleLevel
                });
            }
            grid.ItemsSource = showRole;
        }

        public static void PrintUserGrid(DataGrid grid,Server server)
        {
            ObservableCollection<ShowUser> showUser = new ObservableCollection<ShowUser>();

            List <User> user = server.receivedUser;

            string[] RoleName = new string[server.receivedRole.Count];
            for (int i = 0; i < server.receivedRole.Count; i++)
            {
                RoleName[i]= server.receivedRole[i].RoleName;
            }

            string[] DepartmentName = new string[server.receivedDepartment.Count];
            for (int i = 0; i < server.receivedDepartment.Count; i++)
            {
                DepartmentName[i] = server.receivedDepartment[i].DepartmentName;
            }

            int roleID = 0;
            int depaID = 0;

            for (int i = 0; i < user.Count; i++)
            {
                roleID = user[i].RoleID;
                depaID = user[i].DepartmentID;
                showUser.Add(new ShowUser { ID = user[i].UserID, Имя = user[i].UserName,
                Фамилия = user[i].Surname, Отчество = user[i].Patronymic,
                Телефон = user[i].PhoneNumber, Логин = user[i].UserLogin, Почта = user[i].Email,
                  
                Роль = RoleName[--roleID],
                Отдел = DepartmentName[--depaID],

                Заблокирован = user[i].Blocked});
            }
            grid.ItemsSource = showUser;
        }

    }
}
