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
using static System.Collections.Specialized.BitVector32;
using Client.Resources.Entitys;
using Server.DataBaseFolder.Entitys;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Globalization;
using Client.Classes;
using System.Net.Http;
using System.Runtime.CompilerServices;

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
            HideSideMenu();           
        }

        private void ProjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectsListBox.SelectedIndex != -1)
            {
                Server.UpdateDocuments(ProjectsListBox.SelectedIndex);
            }
            else 
            {
                DocumentsListBox.Items.Clear();
            }
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
                //DropBox[i] = DropBox[i].Remove(DropBox[i].Length - 2, 2);
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
                int nameInPatternID = -1;
                if (DocumentsListBox.SelectedIndex != -1)
                {                    
                    TextBlock block = (TextBlock)DocumentsListBox.SelectedItem;
                    int n =block.Inlines.Count;
                    var runWithNameInPattern = block.Inlines.ElementAtOrDefault(--n) as Run;
                    string nameInPattern = runWithNameInPattern.Text;

                    if (nameInPattern.Contains("\b") || nameInPattern.Contains("\n"))
                    {
                        string docname = DropBox[i].Split("\\").Last();
                        nameInPattern = nameInPattern.StartsWith("\b") ? nameInPattern.Substring(1) : nameInPattern;
                        MessageBoxResult result = MessageBox.Show(
                        $"Хотите добавить файл {docname}\nкак {nameInPattern}", // Текст
                        "Подтверждение",                                  // Заголовок
                        MessageBoxButton.YesNo,                          // Кнопки Да/Нет
                        MessageBoxImage.Question                         // Иконка
                        );
                        // Обработка выбора
                        if (result == MessageBoxResult.Yes)
                        {
                            nameInPatternID = Server.GetNameInPatternID(ProjectsListBox.SelectedIndex, nameInPattern);
                        }
                    }

                }                
                Server.SendDocument(ProjectsListBox.SelectedItem, DropBox[i] , nameInPatternID);
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
            if (DocumentsListBox.SelectedIndex == -1 && ProjectsListBox.SelectedIndex == -1)
            { return; }

            TextBlock block = (TextBlock)DocumentsListBox.SelectedItem;
            string file = block.Text;

            Server.DownloadDocument(ProjectsListBox.SelectedItem, file);
        }

        private void FindProjectButton_Click(object sender, RoutedEventArgs e)
        {

            if (FindProjectTextBox.Text == "")
            {
                FindProjectComboBox.Items.Clear();

            }
            FindProjectComboBox.Items.Clear();
            string[] folder = Server.FindFolder(FindProjectTextBox.Text);
            for (int i = 0; i < folder.Length; i++)
            {
                FindProjectComboBox.Items.Add(folder[i]);
            }
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
            if (ProjectDepartments.Items.Count > 0 && AplyedProjectPattern.SelectedIndex != -1 && FindProjectComboBox.SelectedIndex != -1)
            {
                Server.ChangeProjectProperties(ProjectDepartments.Items.OfType<string>().ToArray(), AplyedProjectPattern.SelectedIndex, FindProjectComboBox.SelectedItem.ToString());
            }
            else { StyleClass.TransactionResult("Все отмеченные поля обязательны для заполнения", this); }
        }
        private void ProjectDepartmentsAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (AllDepartmentsNew.SelectedIndex != -1)
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
            else { StyleClass.TransactionResult("Все отмеченные поля обязательны для заполнения",this); }
        }

        private void HideFileMenu_Click(object sender, RoutedEventArgs e)
        {
            HideSideMenu();
        }

        private void ShowFileMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowSideMenu();
        }
        public void ShowSideMenu()
        {
            LastColumn.Width = new GridLength(1, GridUnitType.Star);
            ShowFileMenu.Visibility = Visibility.Hidden;
            PreLastColumn.Width = new GridLength(3, GridUnitType.Pixel);
            HideFileMenu.Visibility = Visibility.Visible;
        }
        public void HideSideMenu()
        {
            LastColumn.Width = new GridLength(0, GridUnitType.Star);
            ShowFileMenu.Visibility = Visibility.Visible;
            PreLastColumn.Width = new GridLength(1, GridUnitType.Auto);
            HideFileMenu.Visibility = Visibility.Hidden;
        }


        private void ResetSelection_Click(object sender, RoutedEventArgs e)
        {
            DocumentsListBox.SelectedIndex = -1;
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int n = SortBox.SelectedIndex;           
            ProjectsListBox.SelectedIndex = -1;
            switch (n)
            {
                case 0:
                    Server.UnDone();
                    break;
                case 1:
                    Server.TimeSort();
                    break;
                case 2:
                    Server.TimeSortReversed();
                    break;
                case 3:
                    Server.NameSort();
                    break;
                case 4:
                    Server.NameSortReversed();
                    break;
                case 5:
                    Server.ClientSort();
                    break;
                case 6:
                    Server.ClientSortReversed();
                    break;
            }
            InPatternBox.SelectedIndex = 0;
        }

        private void InPatternBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Server.UpdateDocuments(ProjectsListBox.SelectedIndex);
        }

        private void AddNewClient_Click(object sender, RoutedEventArgs e)
        {
            bool isValidEmail = Regex.IsMatch(Email.Text,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);

            if (!isValidEmail == true || Email.Text == null)
            {
                Email.BorderBrush = Brushes.Red;
                return;
            }
            else { Email.BorderBrush = Brushes.Gray; }

            if (!Regex.IsMatch(INN.Text, "^[0-9]{10}$") || INN.Text == "")
            {
                INN.BorderBrush = Brushes.Red;
                return;
            }
            else { Email.BorderBrush = Brushes.Gray; }

            if (!Regex.IsMatch(OGRN.Text, "^[0-9]{13}$") || OGRN.Text == "")
            {
                OGRN.BorderBrush = Brushes.Red;
                return;
            }
            else { Email.BorderBrush = Brushes.Gray; }

            if (!Regex.IsMatch(KPP.Text, "^[0-9]{9}$") || KPP.Text == "")
            {
                KPP.BorderBrush = Brushes.Red;
                return;
            }
            else { Email.BorderBrush = Brushes.Gray; }

            if (ClientName.Text == "")
            {
                ErrorMessage("Поле имени обязательно для заполнения");
                ClientName.BorderBrush = Brushes.Red;
                return;
            }
            else { Email.BorderBrush = Brushes.Gray; }

            if (!Regex.IsMatch(ClientName.Text, @"^(ООО|АО|ПАО)\s+""[^""]+""$"))
            {
                ErrorMessage("Форма для названия ООО/АО/ПАО + \"название организации\"");
                ClientName.BorderBrush = Brushes.Red;
                return;
            }
            else { Email.BorderBrush = Brushes.Gray; }

            string[] ClientInfo = new string[5];
            ClientInfo[0] = ClientName.Text;
            ClientInfo[1] = INN.Text;
            ClientInfo[2] = Email.Text;
            ClientInfo[3] = OGRN.Text;
            ClientInfo[4] = KPP.Text;
            Server.NewOrUpdateClient(ClientInfo);
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            Server.FindClient(ClientNameLabel.Text);
        }

        public void ErrorMessage(string Error)
        {
            Popup1Text.Text = Error;
            Popup1.IsOpen = true;
            Popup1Text.Background = new SolidColorBrush(Colors.LightPink);

            // Таймер для запуска анимации через 2 секунды
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (sender, e) =>
                {
                    Popup1.IsOpen = false;
                    timer.Stop();
                };
             timer.Start();
        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            ClientName.Text = ClientNameLabel.Text;
            INN.Text = INNLabel.Content.ToString();
            Email.Text = EmailLabel.Content.ToString();
            OGRN.Text = OGRNLabel.Content.ToString();
            KPP.Text = KPPLabel.Content.ToString();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StyleClass.MainSizeChanged(sender, e, this);
        }
    }
}
