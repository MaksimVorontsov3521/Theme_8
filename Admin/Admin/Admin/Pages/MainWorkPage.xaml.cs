using Admin.Classes;
using Admin.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace Admin.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainWorkPage.xaml
    /// </summary>
    public partial class MainWorkPage : Page
    {
        Server server;
        public MainWorkPage(Server server)
        {
            InitializeComponent();
            UsersGrid.ItemsSource = server.receivedUser;
            this.server = server;
        }

        private void ConnectionStringButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BaseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            server.UpdateBaseFolder(BaseFolderTB.Text);
        }

        private void ButtonAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            server.AddDepartment(FindDepartment.Text);
        }

        private void ButtonChangeDepartment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {


        }
        private void ButtonAddUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonChangeUser_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonSerializer.Serialize(UsersGrid.SelectedItem);
            User user = JsonSerializer.Deserialize<User>(json);
            if (user == null)
            {
                return;
            }
            ChangeUser changeUser = new ChangeUser(user,server,this);
            UserFrame.Navigate(changeUser);
        }
    }
}
