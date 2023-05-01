using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DummyDatabase.Core;
using static System.Net.Mime.MediaTypeNames;

namespace DummyDatabase.Desktop.windows_for_editing.scheme
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void CreateColumn(object sender, RoutedEventArgs e)
        {
            Grid column = CreateGrid(columnsList.Items.Count);
            columnsList.Items.Add(column);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

        private Grid CreateGrid(int number)
        {
            Grid grid = new Grid();
            grid.Name = $"number{number}";

            for (int i = 0; i < 7; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            TextBlock name = new();
            name.Text = "Имя столбца:";
            TextBox ColumnName = new();
            ColumnName.Width = 100;

            TextBlock type = new();
            type.Text = "Тип столбца:";
            TextBox ColumnType = new();
            ColumnType.Width = 100;

            TextBlock primary = new();
            primary.Text = "Это главный столбец?";
            CheckBox isPrimary = new();
            isPrimary.Width = 30;

            Button deleteButton = new();
            deleteButton.Width = 60;
            deleteButton.Content = "Удалить";
            deleteButton.Click += DeleteColumn;

            grid.Children.Add(name);
            grid.Children.Add(ColumnName);
            grid.Children.Add(type);
            grid.Children.Add(ColumnType);
            grid.Children.Add(primary);
            grid.Children.Add(isPrimary);
            grid.Children.Add(deleteButton);

            Grid.SetColumn(name, 0);
            Grid.SetColumn(ColumnName, 1);
            Grid.SetColumn(type, 2);
            Grid.SetColumn(ColumnType, 3);
            Grid.SetColumn(primary, 4);
            Grid.SetColumn(isPrimary, 5);
            Grid.SetColumn(deleteButton, 6);

            return grid;
        }

        private void DeleteColumn(object sender, RoutedEventArgs e)
        {
            //тут должно быть удаление столбца но я пока не разобрался
        }

        private void CreateScheme(object sender, RoutedEventArgs e)
        {
            Scheme newScheme = new();

            if(IsAbleToCreate())
            {
                newScheme.Name = schemeName.Text;

                List<SchemeColumn> columns = new();

                var gridColumns = columnsList.Items;
                foreach (Grid gridColumn in gridColumns)
                {
                    TextBox name = (TextBox)gridColumn.Children[1];
                    TextBox type = (TextBox)gridColumn.Children[3];
                    CheckBox isPrimary = (CheckBox)gridColumn.Children[5];

                    if(isPrimary.IsChecked == true)
                    {
                        columns.Add(new SchemeColumn(name.Text, type.Text, true));
                    }
                    else
                    {
                        columns.Add(new SchemeColumn(name.Text, type.Text, false));
                    }
                }
                newScheme.Columns = columns.ToArray();

                string s = JsonSerializer.Serialize(newScheme);

                string path = $"{newScheme.Name}.txt";


                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    writer.Write(s);
                }
            }
            else
            {
                MessageBox.Show("aboba");
            }
        }

        private bool IsAbleToCreate()
        {
            if (schemeName.Text != "")
            {
                var gridColumns = columnsList.Items;
                bool IsAbleToCreate = true;
                int countPrimaryColumns = 0;

                foreach (Grid gridColumn in gridColumns)
                {
                    TextBox name = (TextBox)gridColumn.Children[1];
                    if(name.Text == "")
                    {
                        IsAbleToCreate = false;
                        break;
                    }

                    TextBox type = (TextBox)gridColumn.Children[3];
                    switch(type.Text)
                    {
                        case "int":
                            break;
                        case "float":
                            break;
                        case "double":
                            break;
                        case "bool":
                            break;
                        case "dateTime":
                            break;
                        case "string":
                            break;
                        default:
                            IsAbleToCreate = false;
                            break;
                    }

                    CheckBox isPrimary = (CheckBox)gridColumn.Children[5];
                    if(isPrimary.IsChecked == true)
                    {
                        countPrimaryColumns++;
                    }
                }

                if(countPrimaryColumns != 1 || gridColumns.Count < 1)
                {
                    IsAbleToCreate = false;
                }

                return IsAbleToCreate;
            }
            else
            {
                return false;
            }
        }
    }
}
