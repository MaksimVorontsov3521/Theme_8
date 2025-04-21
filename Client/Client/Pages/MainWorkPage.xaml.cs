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
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Client.Resources;
using System.Windows.Markup;

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
            Server.UpdateDocuments(ProjectsListBox.SelectedIndex);
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (ProjectsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите проект");
                return;
            }

            string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (items.Length != files.Length)
            {
                MessageBox.Show("Извините, папки не поддерживаются.",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Проверка на наличие папок
                bool hasFolders = files.Any(item =>
                {
                    try
                    {
                        return Directory.Exists(item);
                    }
                    catch
                    {
                        return false;
                    }
                });

                e.Effects = hasFolders ? DragDropEffects.None : DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

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
            string[] DropBox = new string[DropBoxLB.Items.Count];
            for (int i = 0; i < DropBoxLB.Items.Count; i++)
            {
                DropBox[i] = DropBoxLB.Items[i].ToString();
                int a = DropBox[i].Length;
                DropBox[i] = DropBox[i].Remove(DropBox[i].Length - 2, 2);
            }
            if (ProjectsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите проект");
                return;
            }
            if (DropBox.Length == 0)
            {
                MessageBox.Show("Выберите файлы");
                return;
            }



            for (int i = 0; i < DropBox.Length; i++)
            {
                DropBoxLB.SelectedIndex = i;
                if (Server.IsDocumentNew(ProjectsListBox.SelectedIndex, DropBox[i]) == false)
                {
                    string docname = DropBox[i].Split("\\").Last();
                    MessageBoxResult result = MessageBox.Show(
                    $"Файл с названием {docname} уже существует.\n Вы хотите его заменить?", // Текст
                    "Подтверждение",                                  // Заголовок
                    MessageBoxButton.YesNo,                          // Кнопки Да/Нет
                    MessageBoxImage.Question                         // Иконка
                    );

                    // Обработка выбора
                    if (result == MessageBoxResult.No)
                    {
                        continue;
                    }                   
                }
                Server.SendDocument(ProjectsListBox.SelectedItem.ToString(), DropBox[i]);         
            }
            DropBoxLB.Items.Clear();
        }

        private void DocumentAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;
                if (System.IO.Directory.Exists(selectedPath))
                {
                    MessageBox.Show("Пожалуйста, выберите файл, а не папку!", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DropBoxLB.Items.Add(openFileDialog.FileName);
            }
        }

        private void DocumentDownload_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsListBox.SelectedIndex == -1 && ProjectsListBox.SelectedIndex==-1)
            { return; }
           
            TextBlock block = (TextBlock)DocumentsListBox.SelectedItem;
            string file = block.Text;
            Server.DownloadDocument(ProjectsListBox.SelectedItem.ToString(), file);
        }
        
        private void FindProjectButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (FindProjectTextBox.Text == "")
            {
                FindProjectComboBox.Items.Clear();

            }
            FindProjectComboBox.Items.Clear();
            string [] folder = Server.FindFolder(FindProjectTextBox.Text);
            for (int i = 0; i < folder.Length; i++)
            {
                FindProjectComboBox.Items.Add(folder[i]);
            }
        }


        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int x = (int)e.NewSize.Width;
            int y = (int)e.NewSize.Height;

            int StandardSize = (x / 100) + (y / 100) + 2;

            SimbolHead1.Width = StandardSize;
            SimbolHead1.Height = StandardSize;

            SimbolHead2.Width = StandardSize;
            SimbolHead2.Height = StandardSize;

            TBHeader1.FontSize = StandardSize;
            TBHeader2.FontSize = StandardSize;
            TBHeader3.FontSize = StandardSize;

            Label1Project.FontSize = StandardSize;
            Label2Project.FontSize = StandardSize;
            Label4Project.FontSize = StandardSize;

            SortBox.FontSize = StandardSize;
            ProjectsListBox.FontSize = StandardSize;

            InPatternBox.FontSize = StandardSize;
            DocumentsListBox.FontSize = StandardSize;

            DocumentDownload.FontSize = StandardSize;
            DocumentAdd.FontSize = StandardSize;
            SendFiles.FontSize = StandardSize;
            DropBoxLB.FontSize = StandardSize;

            FindProjectButton.FontSize = StandardSize;

            //

            Label1Pattern.FontSize = StandardSize;
            Label2Pattern.FontSize = StandardSize;
            Label3Pattern.FontSize = StandardSize;
            Label4Pattern.FontSize = StandardSize;
            Label5Pattern.FontSize = StandardSize;
            Label6Pattern.FontSize = StandardSize;
            Label7Pattern.FontSize = StandardSize;
            Label8Pattern.FontSize = StandardSize;
            Label9Pattern.FontSize = StandardSize;
            Label10Pattern.FontSize = StandardSize;
            Label11Pattern.FontSize = StandardSize;



            FindProjectButton.FontSize = StandardSize;
            FindProjectTextBox.FontSize = StandardSize;
            FindProjectComboBox.FontSize = StandardSize;

            AllDepartments.FontSize = StandardSize;
            ProjectDepartments.FontSize = StandardSize;
            ProjectDepartmentsAdd.FontSize = StandardSize;
            ProjectDepartmentsRemove.FontSize = StandardSize;

            ApplyNewProjectProperties.FontSize = StandardSize;
            AplyedProjectPattern.FontSize = StandardSize;
            NewPattern.FontSize = StandardSize;

            AplyedNewProjectPattern.FontSize = StandardSize;

            AllDepartmentsNew.FontSize = StandardSize;
            ProjectDepartmentsNew.FontSize = StandardSize;
            ProjectDepartmentsAddNew.FontSize = StandardSize;
            ProjectDepartmentsRemoveNew.FontSize = StandardSize;

            AplyedNewProjectPattern.FontSize = StandardSize;
            NewPatternNew.FontSize = StandardSize;

            CreateNewProject.FontSize = StandardSize;
        }

        private void ProjectDepartmentsAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AllDepartments.SelectedIndex != -1)
            {
                ProjectDepartments.Items.Add(AllDepartments.SelectedItem);
                ProjectDepartments.SelectedItem = AllDepartments.SelectedItem.ToString();
                AllDepartments.Items.RemoveAt(AllDepartments.SelectedIndex);
            }
        }

        private void ProjectDepartmentsRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectDepartments.SelectedIndex != -1)
            {
                AllDepartments.Items.Add(ProjectDepartments.SelectedItem);
                ProjectDepartments.Items.RemoveAt(ProjectDepartments.SelectedIndex);
                string[] array = new string[AllDepartments.Items.Count];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = AllDepartments.Items[i].ToString();
                }
                Array.Sort(array, StringComparer.OrdinalIgnoreCase);
                AllDepartments.Items.Clear();
                for (int i = 0; i < array.Length; i++)
                {
                    AllDepartments.Items.Add(array[i]);
                }
            }
        }
        private void ApplyNewProjectProperties_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectDepartments.Items.Count > 0 && AplyedProjectPattern.SelectedIndex != -1 && FindProjectComboBox.SelectedIndex!=-1)
            {
                Server.ChangeProjectProperties(ProjectDepartments.Items.OfType<string>().ToArray(), AplyedProjectPattern.SelectedIndex, FindProjectComboBox.SelectedItem.ToString());
            }
            else { Server.TransactionResult("Все отмеченные поля обязательны для заполнения"); }
        }
        private void ProjectDepartmentsAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (AllDepartmentsNew.SelectedIndex!=-1)
            { 
                ProjectDepartmentsNew.Items.Add(AllDepartmentsNew.SelectedItem);
                ProjectDepartmentsNew.SelectedItem = AllDepartmentsNew.SelectedItem.ToString();
                AllDepartmentsNew.Items.RemoveAt(AllDepartmentsNew.SelectedIndex);
            }       
        }

        private void ProjectDepartmentsRemoveNew_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectDepartmentsNew.SelectedIndex != -1)
            {
                AllDepartmentsNew.Items.Add(ProjectDepartmentsNew.SelectedItem);
                ProjectDepartmentsNew.Items.RemoveAt(ProjectDepartmentsNew.SelectedIndex);
                string[] array = new string[AllDepartmentsNew.Items.Count];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = AllDepartmentsNew.Items[i].ToString();
                }
                Array.Sort(array, StringComparer.OrdinalIgnoreCase);
                AllDepartmentsNew.Items.Clear();
                for (int i = 0; i < array.Length; i++)
                {
                    AllDepartmentsNew.Items.Add(array[i]);
                }
            }
        }

        private void CreateNewProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectDepartmentsNew.Items.Count > 0 && AplyedNewProjectPattern.SelectedIndex != -1 && NewProjectName.Text != "")
            {
                Server.CreateNewProject(ProjectDepartmentsNew.Items.OfType<string>().ToArray(), AplyedNewProjectPattern.SelectedIndex, NewProjectName.Text);
            }
            else { Server.TransactionResult("Все отмеченные поля обязательны для заполнения"); }
        }

    }
}
