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
using System.Security.Cryptography;

namespace Admin.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddNewUser.xaml
    /// </summary>
    public partial class AddNewUser : Page
    {
        User User;
        Server server;
        MainWorkPage Frame;
        public AddNewUser( Server Server, MainWorkPage frame)
        {
            InitializeComponent();
            User user = new User();
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
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Page page = new Page();
            Frame.UserFrame.Navigate(page);
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
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

            User.RoleID = UserRole.SelectedIndex + 1;
            User.DepartmentID = UserDepartment.SelectedIndex + 1;
            User.Email = TBEmail.Text;
            User.Blocked = false;

            int returnCount = 0;

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
                return;
            }
            else { TBEmail.BorderBrush = Brushes.Gray; }

            if (UserRole.SelectedIndex == -1)
            { UserRole.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }
            if (UserDepartment.SelectedIndex == -1)
            { UserDepartment.BorderBrush = new SolidColorBrush(Colors.Red); returnCount++; }

            if (returnCount > 0)
            {
                StyleClass.TransactionResult("Все поля обязательны для заполнения", Frame);
                return;
            }

            User.UserPassword = Security.HashPassword(User.UserPassword);

            server.UpdateClient(User);
            Page page = new Page();
            Frame.UserFrame.Navigate(page);
        }

    }
}
