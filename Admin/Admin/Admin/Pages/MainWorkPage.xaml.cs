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
using static Admin.Classes.StyleClass;

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
            this.server = server;
            PrintGrids();
            
        }

        public void PrintGrids()
        {
            StyleClass.PrintUserGrid(UsersGrid, server);
            DepartmentGrid.ItemsSource = server.receivedDepartment;
        }

        private void ConnectionStringButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BaseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            server.UpdateBaseFolder(BaseFolderTB.Text);
            server.PrintGrids(this);
        }

        private void ButtonAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            server.AddDepartment(FindDepartment.Text);
            server.PrintGrids(this);
        }

        private void ButtonAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddNewUser addNewUser = new AddNewUser(server,this);
            UserFrame.Navigate(addNewUser);
        }

        private void ButtonChangeUser_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonSerializer.Serialize(UsersGrid.SelectedItem);
            ShowUser userGrid = JsonSerializer.Deserialize<ShowUser>(json);

            if (userGrid == null)
            {
                return;
            }

            User user = server.receivedUser.FirstOrDefault(u=> u.UserID == userGrid.ID);
            ChangeUser changeUser = new ChangeUser(user,server,this);
            UserFrame.Navigate(changeUser);
        }
    }
}
