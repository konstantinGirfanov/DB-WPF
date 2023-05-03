using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DummyDatabase.Core;

namespace DummyDatabase.Desktop.WindowsForEditing.Scheme
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

        private void CreateGridForColumn(object sender, RoutedEventArgs e)
        {
            Grid gridForColumn = CreateGrid();
            columnsList.Items.Add(gridForColumn);
        }

        private Grid CreateGrid()
        {
            Grid gridForColumn = new Grid();

            for (int i = 0; i < 7; i++)
            {
                gridForColumn.ColumnDefinitions.Add(new ColumnDefinition());
            }

            TextBlock name = new();
            name.Text = "Имя столбца:";
            gridForColumn.Children.Add(name);
            Grid.SetColumn(name, 0);

            TextBox columnName = new();
            columnName.Width = 100;
            gridForColumn.Children.Add(columnName);
            Grid.SetColumn(columnName, 1);

            TextBlock type = new();
            type.Text = "Тип столбца:";
            gridForColumn.Children.Add(type);
            Grid.SetColumn(type, 2);

            TextBox columnType = new();
            columnType.Width = 100;
            gridForColumn.Children.Add(columnType);
            Grid.SetColumn(columnType, 3);

            TextBlock isPrimary = new();
            isPrimary.Text = "Это главный столбец?";
            gridForColumn.Children.Add(isPrimary);
            Grid.SetColumn(isPrimary, 4);

            CheckBox isPrimaryColumn = new();
            isPrimaryColumn.Width = 30;
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

        private void CreateScheme(object sender, RoutedEventArgs e)
        {
            if (IsAbleToCreate())
            {
                Core.Scheme newScheme = new();
                newScheme.Name = schemeName.Text;

                List<SchemeColumn> newSchemeColumns = new();

                ItemCollection columnGrids = columnsList.Items;
                foreach (Grid gridForColumn in columnGrids)
                {
                    TextBox columnName = (TextBox)gridForColumn.Children[1];
                    TextBox columnType = (TextBox)gridForColumn.Children[3];
                    CheckBox isPrimaryColumn = (CheckBox)gridForColumn.Children[5];

                    if (isPrimaryColumn.IsChecked == true)
                    {
                        newSchemeColumns.Add(new SchemeColumn(columnName.Text, columnType.Text, true));
                    }
                    else
                    {
                        newSchemeColumns.Add(new SchemeColumn(columnName.Text, columnType.Text, false));
                    }
                }
                newScheme.Columns = newSchemeColumns.ToArray();

                string schemeJSON = JsonSerializer.Serialize(newScheme);

                string schemesFolderPath = WorkWithFiles.GetFolderPath("schemes");
                string newSchemePath = $"{schemesFolderPath}\\{newScheme.Name}.json";

                if (!File.Exists(newSchemePath))
                {
                    File.WriteAllText(newSchemePath, schemeJSON);

                    // Обновление листбокса в главном окне, который содержит список схем из папки.
                    ((ListBox)((ScrollViewer)((Grid)((Grid)this.Owner.Content).Children[0]).Children[2]).Content).ItemsSource = WorkWithFiles.GetFolderFiles("schemes");

                    MessageBox.Show("Схема добавлена");
                }
                else
                {
                    MessageBox.Show("Схема с таким названием уже существует");
                }
            }
            else
            {
                MessageBox.Show("Ошибка");
            }
        }

        private bool IsAbleToCreate()
        {
            if (schemeName.Text != "")
            {
                List<string> columnNames = new();
                ItemCollection gridColumns = columnsList.Items;
                bool IsAbleToCreate = true;
                int countPrimaryColumns = 0;

                foreach (Grid gridColumn in gridColumns)
                {
                    TextBox columnName = (TextBox)gridColumn.Children[1];
                    if (columnName.Text == "" || columnNames.Contains(columnName.Text))
                    {
                        IsAbleToCreate = false;
                        break;
                    }
                    columnNames.Add(columnName.Text);

                    TextBox columnType = (TextBox)gridColumn.Children[3];
                    switch (columnType.Text)
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

                    CheckBox isPrimaryColumn = (CheckBox)gridColumn.Children[5];
                    if (isPrimaryColumn.IsChecked == true)
                    {
                        countPrimaryColumns++;
                    }
                }

                if (countPrimaryColumns != 1 || gridColumns.Count < 1)
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
        private void ColumnsScrollerScroll(object sender, MouseWheelEventArgs e)
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
    }
}
