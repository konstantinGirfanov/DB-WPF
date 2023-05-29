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
        Core.Scheme currentScheme;

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
            currentScheme = WorkWithScheme.ReadScheme($"{schemesPath}\\{schemeName}");

            schemeNameBox.Text = schemeName;
            foreach (SchemeColumn column in currentScheme.Columns)
            {
                Grid gridForColumn = CreateGridForColumn(column.Name, column.Type, column.IsPrimary, column.ForeignKey);
                columnsList.Items.Add(gridForColumn);
            }
        }

        private Grid CreateGridForColumn(string name, string type, bool isPrimary, ForeignKey foreignKey)
        {
            Grid gridForColumn = new();

            for (int i = 0; i < 9; i++)
            {
                gridForColumn.ColumnDefinitions.Add(new ColumnDefinition());
            }

            gridForColumn.RowDefinitions.Add(new RowDefinition());
            gridForColumn.RowDefinitions.Add(new RowDefinition());
            gridForColumn.RowDefinitions.Add(new RowDefinition());

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

            TextBlock foreignKeyTextBlock = new();
            isPrimaryTextBlock.Text = "Внешний ключ: ";
            gridForColumn.Children.Add(foreignKeyTextBlock);
            Grid.SetColumn(isPrimaryTextBlock, 6);

            CheckBox foreignKeyCheckBox = new();
            if (foreignKey != null)
            {
                foreignKeyCheckBox.IsChecked = true;
                gridForColumn.Children.Add(foreignKeyCheckBox);
                Grid.SetColumn(foreignKeyCheckBox, 7);

                ListBox schemesListBox = new();
                LoadForeignKey(schemesListBox);

                gridForColumn.Children.Add(schemesListBox);
                Grid.SetRow(schemesListBox, 1);

                TextBlock foreignKeyInfo = new();
                foreignKeyInfo.Text = $"Привязка: {foreignKey.Scheme.Name}.{foreignKey.SchemeColumn.Name}";
                gridForColumn.Children.Add(foreignKeyInfo);
                Grid.SetRow(foreignKeyInfo, 2);
            }
            else
            {
                foreignKeyCheckBox.IsChecked = false;
                gridForColumn.Children.Add(foreignKeyCheckBox);
                Grid.SetColumn(foreignKeyCheckBox, 7);

                TextBlock foreignKeyInfo = new();
                gridForColumn.Children.Add(foreignKeyInfo);
            }
            foreignKeyCheckBox.Click += IsForeignKeyClick;

            Button deleteButton = new();
            deleteButton.Width = 60;
            deleteButton.Content = "Удалить";
            deleteButton.Click += DeleteColumn;
            gridForColumn.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 8);

            return gridForColumn;
        }

        private void DeleteColumn(object sender, RoutedEventArgs e)
        {
            Grid buttonParent = (Grid)((Button)e.Source).Parent;
            columnsList.Items.Remove(buttonParent);
        }

        private void LoadForeignKey(ListBox schemesListBox)
        {
            List<string> files = WorkWithFiles.GetFolderFiles("schemes");

            foreach (string file in files)
            {
                string schemePath = $"{WorkWithFiles.GetFolderPath("schemes")}\\{file}";
                List<string> schemeColumns = WorkWithScheme.ReadScheme(schemePath).GetSchemeColumns();

                ListBox listBoxForSchemeColumns = new();
                listBoxForSchemeColumns.MouseDoubleClick += BindColumn;

                for (int i = 0; i < schemeColumns.Count; i++)
                {
                    listBoxForSchemeColumns.Items.Add(schemeColumns[i]);
                }

                TreeViewItem treeForListBox = new();
                treeForListBox.Header = file;

                treeForListBox.Items.Add(listBoxForSchemeColumns);
                schemesListBox.Items.Add(treeForListBox);
            }
        }

        private void BindColumn(object sender, MouseButtonEventArgs e)
        {
            ListBox box = (ListBox)sender;
            string selectedColumn = box.SelectedItem.ToString().Split(' ')[0];

            string schemeName = ((TreeViewItem)(box.Parent)).Header.ToString();

            int index = ((Grid)((ListBox)((TreeViewItem)box.Parent).Parent).Parent).Children.Count - 2;
            ((TextBlock)((Grid)((ListBox)((TreeViewItem)box.Parent).Parent).Parent).Children[index]).Text = $"Привязка: {schemeName} - {selectedColumn}";
        }

        private void IsForeignKeyClick(object sender, RoutedEventArgs e)
        {
            CheckBox isForeignKey = (CheckBox)e.Source;

            Grid gridWithCheckBox = (Grid)isForeignKey.Parent;

            gridWithCheckBox.RowDefinitions.Add(new RowDefinition());
            gridWithCheckBox.RowDefinitions.Add(new RowDefinition());
            gridWithCheckBox.RowDefinitions.Add(new RowDefinition());

            ListBox schemeColumnsListBox = new();

            if (isForeignKey.IsChecked == true)
            {
                gridWithCheckBox.Children.Remove(gridWithCheckBox.Children[^1]);

                LoadForeignKey(schemeColumnsListBox);
                gridWithCheckBox.Children.Add(schemeColumnsListBox);
                Grid.SetRow(schemeColumnsListBox, 1);

                TextBlock foreignKeyInfo = new();
                foreignKeyInfo.Text = "Привязка:";
                gridWithCheckBox.Children.Add(foreignKeyInfo);
                Grid.SetRow(foreignKeyInfo, 2);

                Button deleteButton = new();
                deleteButton.Width = 60;
                deleteButton.Content = "Удалить";
                deleteButton.Click += DeleteColumn;
                gridWithCheckBox.Children.Add(deleteButton);
                Grid.SetColumn(deleteButton, 8);
            }
            else
            {
                gridWithCheckBox.Children.Remove(gridWithCheckBox.Children[^1]);
                gridWithCheckBox.Children.Remove(gridWithCheckBox.Children[^1]);
            }
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
            columnsList.Items.Add(CreateGridForColumn("", "", false, null));
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

                ReloadMainWindowColumns(scheme);

                MessageBox.Show("Схема обновлена");
            }
            else
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ReloadMainWindowColumns(Core.Scheme scheme)
        {
            ((ListBox)((ScrollViewer)((Grid)Owner.Content).Children[1]).Content).ItemsSource = scheme.GetSchemeColumns();
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
                CheckBox isForeignKey = (CheckBox)gridForColumn.Children[7];

                if (isPrimaryColumn.IsChecked == true)
                {
                    if (isForeignKey != null)
                    {
                        ForeignKey key = CreateForeignKey(gridForColumn);
                        newSchemeColumns.Add(new SchemeColumn(columnName.Text, columnType.Text, true, key));
                    }
                    else
                    {

                        newSchemeColumns.Add(new SchemeColumn(columnName.Text, columnType.Text, true));
                    }
                }
                else
                {
                    if (isForeignKey != null)
                    {
                        ForeignKey key = CreateForeignKey(gridForColumn);
                        newSchemeColumns.Add(new SchemeColumn(columnName.Text, columnType.Text, false, key));
                    }
                    else
                    {

                        newSchemeColumns.Add(new SchemeColumn(columnName.Text, columnType.Text, false));
                    }
                }
            }
            scheme.Columns = newSchemeColumns.ToArray();

            return scheme;
        }

        private ForeignKey? CreateForeignKey(Grid grid)
        {
            int index = grid.Children.Count - 2;
            if (((TextBlock)grid.Children[index]).Text.Split(' ').Length > 1)
            {
                string foreignKeyInfo = ((TextBlock)grid.Children[index]).Text;
                string[] values = foreignKeyInfo.Split("Привязка: ")[1].Split(" - ");

                string schemeName = values[0];
                string schemePath = $"{WorkWithFiles.GetFolderPath("schemes")}\\{schemeName}";

                string columnsName = values[1];

                return new ForeignKey(schemePath, columnsName);
            }
            else
            {
                return null;
            }
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