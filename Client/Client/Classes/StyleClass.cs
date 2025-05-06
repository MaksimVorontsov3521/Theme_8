using Client.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace Client.Classes
{
    internal class StyleClass
    {

        internal static void TransactionResult(string Result, MainWorkPage page)
        {

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

        public static void MainSizeChanged(object sender, SizeChangedEventArgs e, MainWorkPage page)
        {
            int x = (int)e.NewSize.Width;
            int y = (int)e.NewSize.Height;

            double StandardSize = (x / 100) + (y / 100) + 2;

            page.NewProjectName.FontSize = StandardSize;
            page.TBCilent.FontSize = StandardSize;

            page.Label11.FontSize = StandardSize;
            page.Label12.FontSize = StandardSize;
            page.Label13.FontSize = StandardSize;
            page.Label14.FontSize = StandardSize;
            page.Label15.FontSize = StandardSize;
            page.Label16.FontSize = StandardSize;
            page.Label17.FontSize = StandardSize;
            page.Label18.FontSize = StandardSize;
            page.Label19.FontSize = StandardSize;
            page.Label20.FontSize = StandardSize;


            page.ClientName.FontSize = StandardSize;
            page.INN.FontSize = StandardSize;
            page.Email.FontSize = StandardSize;
            page.OGRN.FontSize = StandardSize;
            page.KPP.FontSize = StandardSize;

            page.ClientNameLabel.FontSize = StandardSize;
            page.INNLabel.FontSize = StandardSize;
            page.EmailLabel.FontSize = StandardSize;
            page.OGRNLabel.FontSize = StandardSize;
            page.KPPLabel.FontSize = StandardSize;

            page.Find.FontSize = StandardSize;
            page.Transfer.FontSize = StandardSize;
            page.AddNewClient.FontSize = StandardSize;

            page.ResetSelection.FontSize = StandardSize;
            //
            //
            //


            page.SimbolHead1.Width = StandardSize;
            page.SimbolHead1.Height = StandardSize;

            page.SimbolHead2.Width = StandardSize;
            page.SimbolHead2.Height = StandardSize;

            page.TBHeader1.FontSize = StandardSize;
            page.TBHeader2.FontSize = StandardSize;
            page.TBHeader3.FontSize = StandardSize;

            page.Label1Project.FontSize = StandardSize;
            page.Label2Project.FontSize = StandardSize;
            page.Label4Project.FontSize = StandardSize;

            page.SortBox.FontSize = StandardSize;
            page.ProjectsListBox.FontSize = StandardSize;

            page.InPatternBox.FontSize = StandardSize;
            page.DocumentsListBox.FontSize = StandardSize;

            page.DocumentDownload.FontSize = StandardSize;
            page.DocumentAdd.FontSize = StandardSize;
            page.SendFiles.FontSize = StandardSize;
            page.DropBoxLB.FontSize = StandardSize;

            page.FindProjectButton.FontSize = StandardSize;



            page.Label1Pattern.FontSize = StandardSize;
            page.Label2Pattern.FontSize = StandardSize;
            page.Label4Pattern.FontSize = StandardSize;
            page.Label5Pattern.FontSize = StandardSize;
            page.Label6Pattern.FontSize = StandardSize;
            page.Label7Pattern.FontSize = StandardSize;
            page.Label8Pattern.FontSize = StandardSize;
            page.Label9Pattern.FontSize = StandardSize;
            page.Label10Pattern.FontSize = StandardSize;
            page.Label11Pattern.FontSize = StandardSize;



            page.FindProjectButton.FontSize = StandardSize;
            page.FindProjectTextBox.FontSize = StandardSize;
            page.FindProjectComboBox.FontSize = StandardSize;

            page.AllDepartments.FontSize = StandardSize;
            page.ProjectDepartments.FontSize = StandardSize;
            page.ProjectDepartmentsAdd.FontSize = StandardSize;
            page.ProjectDepartmentsRemove.FontSize = StandardSize;

            page.ApplyNewProjectProperties.FontSize = StandardSize;
            page.AplyedProjectPattern.FontSize = StandardSize;
            page.NewPattern.FontSize = StandardSize;

            page.AplyedNewProjectPattern.FontSize = StandardSize;

            page.AllDepartmentsNew.FontSize = StandardSize;
            page.ProjectDepartmentsNew.FontSize = StandardSize;
            page.ProjectDepartmentsAddNew.FontSize = StandardSize;
            page.ProjectDepartmentsRemoveNew.FontSize = StandardSize;

            page.AplyedNewProjectPattern.FontSize = StandardSize;
            page.NewPatternNew.FontSize = StandardSize;

            page.CreateNewProject.FontSize = StandardSize;
        }

    }
}
