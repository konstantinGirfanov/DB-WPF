using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DummyDatabase.Core;

namespace DummyDatabase.Desktop.WindowsForEditing.Columns
{
    /// <summary>
    /// Логика взаимодействия для Редактирование_таблиц.xaml
    /// </summary>
    public partial class TablesEiditing : Window
    {
        public TablesEiditing()
        {
            InitializeComponent();
            schemeList.ItemsSource = WorkWithFiles.GetFolderFiles("schemes");
        }

        private void LoadSchemeColumns(object sender, MouseButtonEventArgs e)
        {
            columnsList.Items.Clear();

            string schemesPath = WorkWithFiles.GetFolderPath("schemes");
            string schemeName = schemeList.SelectedItem.ToString();
            Core.Scheme scheme = WorkWithScheme.ReadScheme($"{schemesPath}\\{schemeName}");

            schemeNameBox.Text = schemeName;
            foreach (SchemeColumn column in scheme.Columns)
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
            if (e.Delta > 0)
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
            if (IsAbleToCreate())
            {
                Core.Scheme scheme = CreateSchemeFromListBox();
                string schemeJSON = JsonSerializer.Serialize(scheme);

                string schemesFolderPath = WorkWithFiles.GetFolderPath("schemes");
                string newSchemePath = $"{schemesFolderPath}\\{scheme.Name.Replace(".json", "")}.json";

                File.WriteAllText(newSchemePath, schemeJSON);

                // Обновление листбокса в главном окне, который содержит список схем из папки.
                ((ListBox)((ScrollViewer)((Grid)((Grid)this.Owner.Content).Children[0]).Children[2]).Content).ItemsSource = WorkWithFiles.GetFolderFiles("schemes");

                MessageBox.Show("Схема обновлена");
            }
            else
            {
                MessageBox.Show("Ошибка");
            }
        }

        private Core.Scheme CreateSchemeFromListBox()
        {
            Core.Scheme scheme = new();
            scheme.Name = schemeNameBox.Text;

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
            scheme.Columns = newSchemeColumns.ToArray();

            return scheme;
        }

        private bool IsAbleToCreate()
        {
            if (schemeNameBox.Text != "")
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
    }
}