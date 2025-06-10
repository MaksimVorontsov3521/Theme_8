using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Client.Classes.Visual;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для Entrance.xaml
    /// </summary>
    public partial class Entrance : Page
    {
        public Server server;
        public Entrance(Server server)
        {
            this.server = server;
            InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int x = (int)e.NewSize.Width;
            int y = (int)e.NewSize.Height;
            int xy = (x / 100) + (y / 100) + 4;
            Enter.FontSize = xy;
            Login.FontSize = xy;
            Password.FontSize = xy;
            LableAvtorizacia.FontSize = xy;
            log.FontSize = xy;
            pas.FontSize = xy;
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (Login.Text.Length < 3 && Password.Password.ToString().Length < 3)
            {
                StyleClass.TransactionResult("Логин и Пароль не могут быть короче 3 символов", this);
                return;
            }
            string login = Login.Text;
            string password = Password.Password.ToString();
            server.Connection(login, password,this);
        }

        private void Enter_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            server.OpenSettings();
        }
    }
}
