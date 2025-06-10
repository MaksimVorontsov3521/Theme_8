using Client.Resources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            GetSetting();
        }
        MainWindow mainWindow;
        Server Server;
        public SettingsPage(MainWindow MainWindow,Server server)
        {
            InitializeComponent();
            mainWindow = MainWindow;
            Server = server;
            GetSetting();
        }

        public void GetSetting()
        {
            ServerPort.Text = Settings1.Default.ServerPort.ToString();
            ServerURL.Text = Settings1.Default.ServerURL.ToString();
            RootFolder.Text = Settings1.Default.RootFolder.ToString();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int x = (int)e.NewSize.Width;
            int y = (int)e.NewSize.Height;

            double StandardSize = (x / 100) + (y / 100) + 2;

            Label1.FontSize = StandardSize;
            Label2.FontSize = StandardSize; 
            Label3.FontSize = StandardSize;

            ServerPort.FontSize = StandardSize;

            ServerURL.FontSize = StandardSize;    

            RootFolder.FontSize = StandardSize;

            Change.FontSize = StandardSize;
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ServerPort.Text, out int serverPort))
            {
                MessageBox.Show("Порт должен быть числом");
                ServerPort.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                ServerPort.BorderBrush = Brushes.Gray;
            }
            
            if (!Directory.Exists(RootFolder.Text))
            {
                MessageBox.Show("Данной папки не найдено");
                RootFolder.BorderBrush = Brushes.Red;
                return;
            }
            else
            {
                RootFolder.BorderBrush = Brushes.Gray;
            }

            Settings1.Default.ServerPort = serverPort;
            Settings1.Default.ServerURL = ServerURL.Text;
            Settings1.Default.RootFolder = RootFolder.Text;
            Settings1.Default.Save();
            MessageBox.Show("Настройки изменены");

            if (mainWindow != null)
            {
                Entrance entrance = new Entrance(Server);
                mainWindow.WorkPlace.Navigate(entrance);
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                Clipboard.SetText(filePath);
                RootFolder.Text = filePath;
            }
        }
    }
}
