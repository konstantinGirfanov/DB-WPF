﻿using System.Collections.Generic;
using System.ComponentModel;
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

        private void RewriteData(object sender, RoutedEventArgs e)
        {
            string[] dataLines = CreateLinesFromTree();
            bool dataCorrespondsToScheme = true;

            foreach(string line in dataLines)
            {
                dataCorrespondsToScheme &= WorkWithScheme.IsCorrespondsToScheme(currentScheme, line);
            }

            bool isDuplicatesExist = false;
            if(dataCorrespondsToScheme)
            {
                List<string> strings = new();
                foreach (string line in dataLines)
                {
                    isDuplicatesExist = IsDuplicate(currentScheme, line, strings);

                    if(isDuplicatesExist)
                    {
                        break;
                    }

                    strings.Add(line);
                }
            }

            if(dataCorrespondsToScheme && !isDuplicatesExist)
            {
                bool isNeedToCheckForeignKey = false;
                foreach(SchemeColumn column in currentScheme.Columns)
                {
                    if(column.ForeignKey != null)
                    {
                        isNeedToCheckForeignKey = true;
                    }
                }

                if(isNeedToCheckForeignKey)
                {
                    bool foreignKeysIsExist = true;
                    foreach (string line in dataLines)
                    {
                        foreignKeysIsExist &= WorkWithScheme.CheckForeignKey(line, currentScheme);
                    }

                    if(foreignKeysIsExist)
                    {
                        string schemeDataName = WorkWithFiles.GetSchemeDataName(currentScheme.Name);
                        string dataFolderPath = WorkWithFiles.GetFolderPath("data");
                        File.WriteAllLines($"{dataFolderPath}\\{schemeDataName}", dataLines);

                        ReloadMainWindowData($"{dataFolderPath}\\{schemeDataName}");

                        MessageBox.Show("Данные перезаписаны");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка в данных");
                    }
                }
            }
            else
            {
                MessageBox.Show("Ошибка в данных");
            }
        }

        private string[] CreateLinesFromTree()
        {
            StringBuilder sb = new();

            foreach (TreeViewItem item in dataTree.Items)
            {
                for (int i = 0; i < item.Items.Count - 1; i++)
                {
                    if (i + 2 != item.Items.Count)
                    {
                        string columnValue = ((TextBox)((Grid)item.Items[i]).Children[1]).Text;
                        sb.Append($"{columnValue};");
                    }
                    else
                    {
                        Grid gridRow = (Grid)item.Items[i];
                        sb.Append($"{((TextBox)(gridRow.Children[1])).Text}");
                    }
                }

                if (item != dataTree.Items[^1])
                {
                    sb.Append("\r\n");
                }
            }

            return sb.ToString().Split("\r\n");
        }

        private bool IsDuplicate(Scheme scheme, string line, List<string> lines)
        {
            foreach (SchemeColumn column in scheme.Columns)
            {
                if(column.IsPrimary)
                {
                    foreach(string lineValue in lines)
                    {
                        int index = scheme.FindSchemeColumnIndex(column);
                        if (lineValue.Split(';')[index] == line.Split(';')[index])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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
            deleteButton.Click += DeleteRow;
            row.Items.Add(deleteButton);

            return row;
        }

        private void DeleteRow(object sender, RoutedEventArgs e)
        {
            bool canDelete = true;
            Row currentRow = CreateRow((TreeViewItem)((Button)(e.Source)).Parent);
            foreach(string file in WorkWithFiles.GetFolderFiles("schemes"))
            {
                Scheme scheme = WorkWithScheme.ReadScheme(WorkWithFiles.GetFilePath("schemes", file.Replace(".json", "")));
                
                foreach(SchemeColumn column in scheme.Columns)
                {
                    if (column.ForeignKey?.Scheme.Name == currentScheme.Name)
                    {
                        canDelete &= CheckForeignKey(column, scheme, currentRow);
                    }
                }

            }

            if(canDelete)
            {
                TreeViewItem buttonParent = (TreeViewItem)((Button)e.Source).Parent;
                dataTree.Items.Remove(buttonParent);
            }
            else
            {
                MessageBox.Show("Нельзя удалить строку - есть зависящие от неё данные.");
            }
        }

        private Row CreateRow(TreeViewItem item)
        {
            StringBuilder sb = new();

            for (int i = 0; i < item.Items.Count - 1; i++)
            {
                if (i + 2 != item.Items.Count)
                {
                    string columnValue = ((TextBox)((Grid)item.Items[i]).Children[1]).Text;
                    sb.Append($"{columnValue};");
                }
                else
                {
                    Grid gridRow = (Grid)item.Items[i];
                    sb.Append($"{((TextBox)(gridRow.Children[1])).Text}");
                }
            }

            return new Row(currentScheme, sb.ToString());
        }

        private bool CheckForeignKey(SchemeColumn column, Scheme scheme, Row currentRow)
        {
            string currentDataName = WorkWithFiles.GetSchemeDataName(currentScheme.Name);
            string currentSchemeDataPath = WorkWithFiles.GetFilePath("data", currentDataName);
            SchemeData currentData = new(currentScheme, currentSchemeDataPath);

            string dataName = WorkWithFiles.GetSchemeDataName(scheme.Name);
            string schemeDataPath = WorkWithFiles.GetFilePath("data", dataName);
            SchemeData data = new(scheme, schemeDataPath);

            foreach(Row row in data.Rows)
            {
                foreach(KeyValuePair<SchemeColumn, object> pair in row.Data)
                {
                    if(pair.Key.Type == column.Type)
                    {
                        if (pair.Value.ToString() == currentRow.Data[column])
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void AddEmptyDataRow(object sender, RoutedEventArgs e)
        {
            if(currentScheme != null)
            {
                Row emptyRow = Row.CreateEmptyRowData(currentScheme);
                dataTree.Items.Add(CreateDataRowTreeItem(emptyRow));
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
