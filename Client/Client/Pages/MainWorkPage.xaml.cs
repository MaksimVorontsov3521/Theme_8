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
using System.IO;
using Microsoft.Win32;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainWorkPage.xaml
    /// </summary>
    public partial class MainWorkPage : Page
    {
        Server Server;
        public MainWorkPage(Server server)
        {
            InitializeComponent();
            Server = server;
        }

        private void ProjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Server.UpdateDocuments(this,ProjectsListBox.SelectedIndex);
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (ProjectsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите проект");
                return;
            }

            foreach (string file in files)
            {
                DropBoxLB.Items.Add(file + Environment.NewLine);
            }

            // Пример обработки первого файла:
            if (files.Length > 0)
            {
                string filePath = files[0];
                string fileName = System.IO.Path.GetFileName(filePath);               
            }
        }

        private void Page_DragEnter(object sender, DragEventArgs e)
        {
            // Проверяем, что перетаскиваются файлы
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void DropBoxLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DropBoxLB.SelectedIndex == -1)
            { return; }
            DropBoxLB.Items.RemoveAt(DropBoxLB.SelectedIndex);
        }

        private void SendFilesToFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocumentAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                DropBoxLB.Items.Add(openFileDialog.FileName);
            }
        }

        private void DocumentDownload_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsListBox.SelectedIndex == -1 && ProjectsListBox.SelectedIndex==-1)
            { return; }

            Server.DownloadDocument(ProjectsListBox.SelectedItem.ToString(),DocumentsListBox.SelectedItem.ToString()+ ".docx");
        }
    }
}
