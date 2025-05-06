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
            ServerPortButton.FontSize = StandardSize;

            ServerURL.FontSize = StandardSize;
            ServerURLButton.FontSize = StandardSize;

            RootFolder.FontSize = StandardSize;
            RootFolderButton.FontSize = StandardSize;
        }
    }
}
