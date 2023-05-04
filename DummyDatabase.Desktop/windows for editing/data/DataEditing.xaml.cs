using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DummyDatabase.Core;

namespace DummyDatabase.Desktop.windows_for_editing.columns
{
    /// <summary>
    /// Логика взаимодействия для DataEditing.xaml
    /// </summary>
    public partial class DataEditing : Window
    {
        Scheme currentScheme;

        public DataEditing()
        {
            InitializeComponent();
            schemeList.ItemsSource = WorkWithFiles.GetFolderFiles("schemes");
        }

        private void LoadDataForScheme(object sender, MouseButtonEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string schemesPath = WorkWithFiles.GetFolderPath("schemes");
            string schemeName = schemeList.SelectedItem.ToString();
            currentScheme = WorkWithScheme.ReadScheme($"{schemesPath}\\{schemeName}");

            string schemeDataName = WorkWithFiles.GetSchemeDataName(currentScheme.Name);
            string dataFolderPath = WorkWithFiles.GetFolderPath("data");

            dataTree.Items.Clear();
            if (File.Exists($"{dataFolderPath}\\{schemeDataName}"))
            {
                List<Row> dataList = new SchemeData(currentScheme, $"{dataFolderPath}\\{schemeDataName}").Rows;

                foreach (Row row in dataList)
                {
                    dataTree.Items.Add(CreateDataRowTreeItem(row));
                }
            }
        }

        private void UpdateData(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new();
            foreach (TreeViewItem item in dataTree.Items)
            {
                for (int i = 0; i < item.Items.Count - 1; i++)
                {
                    if (i + 2 != item.Items.Count)
                    {
                        Grid gridRow = (Grid)item.Items[i];
                        sb.Append($"{((TextBox)(gridRow.Children[1])).Text};");
                    }
                    else
                    {
                        Grid gridRow = (Grid)item.Items[i];
                        sb.Append($"{((TextBox)(gridRow.Children[1])).Text}");
                    }
                }
                sb.AppendLine();
            }

            string schemeDataName = WorkWithFiles.GetSchemeDataName(currentScheme.Name);
            string dataFolderPath = WorkWithFiles.GetFolderPath("data");
            File.WriteAllText($"{dataFolderPath}\\{schemeDataName}", sb.ToString());

            ReloadMainWindowData($"{dataFolderPath}\\{schemeDataName}");

            MessageBox.Show("Данные перезаписаны");
        }

        private void ReloadMainWindowData(string dataPath)
        {
            if (File.Exists(dataPath))
            {
                ((ListBox)((ScrollViewer)((Grid)this.Owner.Content).Children[3]).Content).ItemsSource = new SchemeData(currentScheme, dataPath).Rows;
            }
            else
            {
                ((ListBox)((ScrollViewer)((Grid)this.Owner.Content).Children[3]).Content).ItemsSource = new List<string>() { "Данные не найдены." };
            }
        }
        private TreeViewItem CreateDataRowTreeItem(Row dataRow)
        {
            TreeViewItem row = new();
            row.Header = $"Cтрока данных";

            foreach (KeyValuePair<SchemeColumn, object> pair in dataRow.Data)
            {
                Grid rowGrid = new();
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock rowColumnInfo = new();
                rowColumnInfo.Text = $"{pair.Key.Name} - {pair.Key.Type}:";
                rowGrid.Children.Add(rowColumnInfo);
                Grid.SetColumn(rowColumnInfo, 0);

                TextBox rowColumnValue = new();
                rowColumnValue.MinWidth = 30;
                rowColumnValue.Text = pair.Value.ToString();
                rowGrid.Children.Add(rowColumnValue);
                Grid.SetColumn(rowColumnValue, 1);

                row.Items.Add(rowGrid);
            }

            Button deleteButton = new();
            deleteButton.Width = 60;
            deleteButton.Content = "Удалить";
            deleteButton.Click += DeleteColumn;
            row.Items.Add(deleteButton);

            return row;
        }

        private void DeleteColumn(object sender, RoutedEventArgs e)
        {
            TreeViewItem buttonParent = (TreeViewItem)((Button)e.Source).Parent;
            dataTree.Items.Remove(buttonParent);
        }

        private void AddEmptyDataRow(object sender, RoutedEventArgs e)
        {
            Row emptyRow = Row.CreateEmptyRowData(currentScheme);
            dataTree.Items.Add(CreateDataRowTreeItem(emptyRow));
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

        private void ScrollDataListScroller(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                dataScroller.LineUp();
            }
            else
            {
                dataScroller.LineDown();
            }
        }
    }
}
