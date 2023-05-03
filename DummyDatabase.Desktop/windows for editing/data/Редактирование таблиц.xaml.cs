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
using System.Windows.Shapes;
using DummyDatabase.Core;

namespace DummyDatabase.Desktop.windows_for_editing.data
{
    /// <summary>
    /// Логика взаимодействия для Редактирование_таблиц.xaml
    /// </summary>
    public partial class Редактирование_таблиц : Window
    {
        public Редактирование_таблиц()
        {
            InitializeComponent();
            schemeList.ItemsSource = WorkWithFiles.GetFolderFiles("schemes");
        }

        private void LoadSchemeColumns(object sender, MouseButtonEventArgs e)
        {
            columnsList.Items.Clear();

            string schemesPath = WorkWithFiles.GetFolderPath("schemes");
            string schemeName = schemeList.SelectedItem.ToString();
            Scheme scheme = WorkWithScheme.ReadScheme($"{schemesPath}\\{schemeName}");

            schemeNameBox.Text = schemeName;
            foreach(SchemeColumn column in scheme.Columns)
            {
                Grid gridForColumn = CreateGridForColumn(column.Name, column.Type, column.IsPrimary);
                columnsList.Items.Add(gridForColumn);
            }
        }

        private Grid CreateGridForColumn(string name, string type, bool isPrimary)
        {
            Grid gridForColumn = new Grid();

            for (int i = 0; i < 7; i++)
            {
                gridForColumn.ColumnDefinitions.Add(new ColumnDefinition());
            }

            TextBlock nameTextBlock = new();
            nameTextBlock.Text = "Имя столбца:";
            gridForColumn.Children.Add(nameTextBlock);
            Grid.SetColumn(nameTextBlock, 0);

            TextBox columnName = new();
            columnName.Width = 100;
            columnName.Text = name;
            gridForColumn.Children.Add(columnName);
            Grid.SetColumn(columnName, 1);

            TextBlock typeTextBlock = new();
            typeTextBlock.Text = "Тип столбца:";
            gridForColumn.Children.Add(typeTextBlock);
            Grid.SetColumn(typeTextBlock, 2);

            TextBox columnType = new();
            columnType.Width = 100;
            columnType.Text = type;
            gridForColumn.Children.Add(columnType);
            Grid.SetColumn(columnType, 3);

            TextBlock isPrimaryTextBlock = new();
            isPrimaryTextBlock.Text = "Это главный столбец?";
            gridForColumn.Children.Add(isPrimaryTextBlock);
            Grid.SetColumn(isPrimaryTextBlock, 4);

            CheckBox isPrimaryColumn = new();
            isPrimaryColumn.Width = 30;
            if (isPrimary)
            {
                isPrimaryColumn.IsChecked = true;
            }
            else
            {
                isPrimaryColumn.IsChecked = false;
            }
            gridForColumn.Children.Add(isPrimaryColumn);
            Grid.SetColumn(isPrimaryColumn, 5);

            Button deleteButton = new();
            deleteButton.Width = 60;
            deleteButton.Content = "Удалить";
            deleteButton.Click += DeleteColumn;
            gridForColumn.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 6);

            return gridForColumn;
        }

        private void DeleteColumn(object sender, RoutedEventArgs e)
        {
            Grid buttonParent = (Grid)((Button)e.Source).Parent;
            columnsList.Items.Remove(buttonParent);
        }

        private void ScrollSchemeListScroller(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                schemeListScroller.LineUp();
            }
            else
            {
                schemeListScroller.LineDown();
            }
        }

        private void ScrollColumnsScroller(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                columnsScroller.LineUp();
            }
            else
            {
                columnsScroller.LineDown();
            }
        }

        private void CreateEmptyGridForColumn(object sender, RoutedEventArgs e)
        {
            columnsList.Items.Add(CreateGridForColumn("", "", false));
        }
        private void OverwriteScheme(object sender, RoutedEventArgs e)
        {

        }
    }
}