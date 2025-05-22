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
            StyleClass.PrintRoleGrid(RolesDataGird,server);

            BaseFolderTB.Text = Settings1.Default.BaseFolder;
            AdminPortTB.Text =Settings1.Default.AdminPort.ToString();
            UsersPortTB.Text = Settings1.Default.UserPort.ToString();
            ServerIP.Text =Settings1.Default.ServerUrl;
            CreateClient.Text = Settings1.Default.CanEditClient.ToString();
            CreateProject.Text = Settings1.Default.CanCreateNewProject.ToString();
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

        private void RolesDataGird_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RolesDataGird.SelectedIndex == -1)
            {
                RoleNameTB.Text = string.Empty; ;
                RoleRightsTB.Text = string.Empty;
                RoleLevelTB.Text = string.Empty;
            }
            if (RolesDataGird.SelectedItem is Role selectedRole)
            {
                RoleNameTB.Text = selectedRole.RoleName;
                RoleRightsTB.Text =selectedRole.Rights;
                RoleLevelTB.Text = selectedRole.RoleLevel.ToString();
            }
            
        }

        private void CreateNewRole_Click(object sender, RoutedEventArgs e)
        {
            Role role = new Role();
            if (RolesDataGird.SelectedIndex == -1)
            {
                try
                {
                    role.RoleLevel = Convert.ToInt32(RoleLevelTB.Text);
                }
                catch { MessageBox.Show("Уровень - это только целые числа, цифрами");return; }

                if (string.IsNullOrEmpty(RoleNameTB.Text))
                { StyleClass.TransactionResult("Название обязательно",this);
                    return;
                }
                role.RoleName = RoleNameTB.Text;
                role.Rights = RoleRightsTB.Text;
                server.CreateNewRole(role);
            }
            else
            {
                if (RolesDataGird.SelectedItem is Role selectedRole)
                {
                    selectedRole.RoleName = RoleNameTB.Text ;
                    selectedRole.Rights = RoleRightsTB.Text ;
                    try
                    { selectedRole.RoleLevel = Convert.ToInt32(RoleLevelTB.Text); }
                    catch
                    {
                        MessageBox.Show("Уровень - это только целые числа, цифрами"); return;
                    }
                    server.CreateNewRole(selectedRole);              
                }
            }
            server.PrintGrids(this);
        }
    }
}
