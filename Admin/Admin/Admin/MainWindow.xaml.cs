using Admin.Classes;
using Admin.Pages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindow mainWindow = this;
            Server server = new Server(mainWindow);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int x = (int)e.NewSize.Width;
            int y = (int)e.NewSize.Height;
            OOO.FontSize = (x / 100) + (y / 100) + 2;
        }
        public void MainWorkPage(MainWorkPage mainWorkPage)
        {
            WorkPlace.Navigate(mainWorkPage);
        }
    }
}