using Admin.Classes;
using Admin.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для ChangeLocalSettings.xaml
    /// </summary>
    public partial class ChangeLocalSettings : Page
    {
        public ChangeLocalSettings()
        {
            InitializeComponent();
            AdminPort.Text = Settings1.Default.AdminPort.ToString();
            serverUrl.Text = Settings1.Default.ServerUrl.ToString();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings1.Default.AdminPort = int.Parse(AdminPort.Text);
                Settings1.Default.ServerUrl = serverUrl.Text;
                Settings1.Default.Save();
                MessageBox.Show("Успешно");
            }
            catch
            { MessageBox.Show("Некорректные значения"); }
            
        }
    }
}
