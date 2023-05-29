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
    public partial class CreatingNewDB : Window
    {
        public CreatingNewDB()
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
            Grid gridForColumn = new();

            for (int i = 0; i < 9; i++)
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

            TextBlock isForeignKeyTextBlock = new();
            isForeignKeyTextBlock.Text = "Внешний ключ?";
            gridForColumn.Children.Add(isForeignKeyTextBlock);
            Grid.SetColumn(isForeignKeyTextBlock, 6);

            CheckBox isForeignKey = new();
            isForeignKey.Width = 30;
            isForeignKey.Click += IsForeignKeyClick;
            gridForColumn.Children.Add(isForeignKey);
            Grid.SetColumn(isForeignKey, 7);

            Button deleteButton = new();
            deleteButton.Width = 60;
            deleteButton.Content = "Удалить";
            deleteButton.Click += DeleteColumn;
            gridForColumn.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 8);

            return gridForColumn;
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
                LoadSchemesIntoList(schemeColumnsListBox);
                gridWithCheckBox.Children.Add(schemeColumnsListBox);
                Grid.SetRow(schemeColumnsListBox, 1);

                TextBlock foreignKeyInfo = new();
                foreignKeyInfo.Text = "Привязка:";
                gridWithCheckBox.Children.Add(foreignKeyInfo);
                Grid.SetRow(foreignKeyInfo, 2);
            }
            else
            {
                gridWithCheckBox.Children.Remove(gridWithCheckBox.Children[^1]);
                gridWithCheckBox.Children.Remove(gridWithCheckBox.Children[^1]);
            }
        }

        private void LoadSchemesIntoList(ListBox schemesListBox)
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

            ((TextBlock)((Grid)((ListBox)((TreeViewItem)box.Parent).Parent).Parent).Children[^1]).Text = $"Привязка: {schemeName} - {selectedColumn}";
        }

        private void DeleteColumn(object sender, RoutedEventArgs e)
        {
            Grid buttonParent = (Grid)((Button)e.Source).Parent;
            columnsList.Items.Remove(buttonParent);
        }

        private void AddNewSchemeToFolder(object sender, RoutedEventArgs e)
        {
            if (IsAbleToCreate())
            {
                Core.Scheme newScheme = CreateSchemeFromListBox();
                string schemeJSON = JsonSerializer.Serialize(newScheme);

                string schemesFolderPath = WorkWithFiles.GetFolderPath("schemes");
                string newSchemePath = $"{schemesFolderPath}\\{newScheme.Name}.json";

                if (!File.Exists(newSchemePath))
                {
                    File.WriteAllText(newSchemePath, schemeJSON);
                    ReloadMainWindowSchemeList();
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

        private void ReloadMainWindowSchemeList()
        {
            ((ListBox)((ScrollViewer)((Grid)((Grid)Owner.Content).Children[0]).Children[2]).Content).ItemsSource = WorkWithFiles.GetFolderFiles("schemes");

        }

        private Core.Scheme CreateSchemeFromListBox()
        {
            Core.Scheme scheme = new();
            scheme.Name = schemeName.Text;

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
                    if (isForeignKey.IsChecked == true)
                    {
                        ForeignKey foreignKey = CreateForeignKey(gridForColumn);
                        newSchemeColumns.Add(new SchemeColumn(columnName.Text, columnType.Text, true, foreignKey));
                    }
                    else
                    {
                        newSchemeColumns.Add(new SchemeColumn(columnName.Text, columnType.Text, true));
                    }
                }
                else
                {
                    if (isForeignKey.IsChecked == true)
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
            if (((TextBlock)grid.Children[^1]).Text.Split(' ').Length > 0)
            {
                string[] foreignKeyInfo = ((TextBlock)grid.Children[^1]).Text
                    .Split("Привязка: ")[1].Split(" - ");

                string schemeName = foreignKeyInfo[0];
                string schemePath = $"{WorkWithFiles.GetFolderPath("schemes")}\\{schemeName}";

                string columnsName = foreignKeyInfo[1];

                return new ForeignKey(schemePath, columnsName);
            }
            else
            {
                return null;
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
