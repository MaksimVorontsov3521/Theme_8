using Admin.Classes;
using Admin.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Admin.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChangeUser.xaml
    /// </summary>
    public partial class ChangeUser : Page
    {
        User User;
        Server server;
        MainWorkPage Frame;
        internal ChangeUser(User user,Server Server,MainWorkPage frame)
        { 
            InitializeComponent();
            Frame = frame;
            server = Server;
            User = user;
            for (int i = 0; i < server.receivedRole.Count; i++)
            {
                UserRole.Items.Add(server.receivedRole[i].RoleName);
            }

            for (int i = 0; i < server.receivedDepartment.Count; i++)
            {
                UserDepartment.Items.Add(server.receivedDepartment[i].DepartmentName);
            }

            Firtsname.Text = user.UserName;
            SecondName.Text = user.Surname;
            Patronymic.Text = user.Patronymic;

            PhoneNumber.Text = user.PhoneNumber;
            UserLogin.Text = user.UserLogin;
            UserPassword.Text = user.UserPassword;

            UserRole.SelectedIndex = user.RoleID-1;
            UserDepartment.SelectedIndex = user.DepartmentID-1;
            TBEmail.Text = user.Email;

            if (user.Blocked == false)
            {Blocked.IsChecked = false;}
            else{ Blocked.IsChecked = true;}
        }

        private void ChangeUserButton_Click(object sender, RoutedEventArgs e)
        {
            Firtsname.BorderBrush = new SolidColorBrush(Colors.Gray);
            SecondName.BorderBrush = new SolidColorBrush(Colors.Gray);
            PhoneNumber.BorderBrush = new SolidColorBrush(Colors.Gray);
            UserLogin.BorderBrush = new SolidColorBrush(Colors.Gray);
            UserPassword.BorderBrush = new SolidColorBrush(Colors.Gray);
            TBEmail.BorderBrush = new SolidColorBrush(Colors.Gray);

            User.UserName = Firtsname.Text;
            User.Surname = SecondName.Text;
            User.Patronymic = Patronymic.Text;

            User.PhoneNumber = PhoneNumber.Text;
            User.UserLogin = UserLogin.Text;
            User.UserPassword = UserPassword.Text;

            User.RoleID = UserRole.SelectedIndex+1;
            User.DepartmentID = UserDepartment.SelectedIndex+1;
            User.Email = TBEmail.Text;

            int returnCount = 0;

            if (Blocked.IsChecked == false)
            { User.Blocked = false; }
            else { User.Blocked = true; }

            if (string.IsNullOrEmpty(Firtsname.Text))
            { Firtsname.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }
            if (string.IsNullOrEmpty(SecondName.Text))
            { SecondName.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }

            if (string.IsNullOrEmpty(PhoneNumber.Text))
            { PhoneNumber.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }
            if (string.IsNullOrEmpty(UserLogin.Text))
            { UserLogin.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }
            if (string.IsNullOrEmpty(UserPassword.Text))
            { UserPassword.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }

            if (string.IsNullOrEmpty(TBEmail.Text))
            { TBEmail.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }


            bool isValidEmail = Regex.IsMatch(TBEmail.Text,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);

            if (!isValidEmail == true || TBEmail.Text == null)
            {
                TBEmail.BorderBrush = Brushes.Red;
                StyleClass.TransactionResult("Введите корректную почту", Frame);
                return;
            }
            else { TBEmail.BorderBrush = Brushes.Gray; }

            if (UserRole.SelectedIndex == -1)
            { UserRole.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }
            if (UserDepartment.SelectedIndex == -1)
            { UserDepartment.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }

            if (returnCount > 0)
            {
                StyleClass.TransactionResult("Все поля обязательны для заполнения",Frame);
                return;         
            }

            User.UserPassword = Security.HashPassword(User.UserPassword);

            server.UpdateClient(User);
            Page page = new Page();
            server.PrintGrids(Frame);
            Frame.UserFrame.Navigate(page);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Page page = new Page();
            Frame.UserFrame.Navigate(page);
        }
    }
}
