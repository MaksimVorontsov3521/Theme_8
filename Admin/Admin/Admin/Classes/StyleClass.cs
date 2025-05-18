using Admin.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace Admin.Classes
{
    internal class StyleClass
    {
        public static void TransactionResult(string Result, MainWorkPage page)
        {
            //
            if (Result.Contains("Успешно"))
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightPink);
            }

            // Таймер для запуска анимации через 2 секунды
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (sender, e) =>
            {
                page.Popup1.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        public static void TransactionResult(string Result, MainWindow page)
        {
            //
            if (Result.Contains("Успешно"))
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                page.Popup1Text.Text = Result;
                page.Popup1.IsOpen = true;
                page.Popup1Text.Background = new SolidColorBrush(Colors.LightPink);
            }

            // Таймер для запуска анимации через 2 секунды
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (sender, e) =>
            {
                page.Popup1.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }
    }
}
