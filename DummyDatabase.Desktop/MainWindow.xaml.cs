using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DummyDatabase.Core;
using DummyDatabase.Desktop.windows_for_editing.columns;
using DummyDatabase.Desktop.WindowsForEditing.Columns;
using DummyDatabase.Desktop.WindowsForEditing.Scheme;

namespace DummyDatabase.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        Scheme scheme;

        public MainWindow()
        {
            InitializeComponent();
            schemeList.ItemsSource = WorkWithFiles.GetFolderFiles("schemes");
        }

        private void LoadScheme(object sender, MouseButtonEventArgs e)
        {
            LoadColumns();
            LoadData();
        }

        private void LoadColumns()
        {
            string schemesPath = WorkWithFiles.GetFolderPath("schemes");
            string schemeName = schemeList.SelectedItem.ToString();
            scheme = WorkWithScheme.ReadScheme($"{schemesPath}\\{schemeName}");

            schemeColumnsList.ItemsSource = scheme.GetSchemeColumns();
        }

        private void LoadData()
        {
            string schemeDataName = WorkWithFiles.GetSchemeDataName(scheme.Name);
            string dataFolderPath = WorkWithFiles.GetFolderPath("data");
            if (File.Exists($"{dataFolderPath}\\{schemeDataName}"))
            {
                schemeDataRows.Items.Clear();
                schemeDataRows.ItemsSource = null;

                var data = new SchemeData(scheme, $"{dataFolderPath}\\{schemeDataName}").Rows;
                if(data.Count != 0)
                {
                    foreach (Row row in data)
                    {
                        schemeDataRows.Items.Add(CreateGridForDataRow(row));
                    }

                    for (int i = 0; i < ((Grid)schemeDataRows.Items[0]).Children.Count; i++)
                    {
                        foreach (Grid item in schemeDataRows.Items)
                        {
                            ((TextBox)item.Children[i]).MinWidth = GetMaxWidthInGridColumn(schemeDataRows.Items, i);
                        }
                    }
                }
                else
                {
                    schemeDataRows.ItemsSource = new List<string>() { "Данные не найдены." };
                }
            }
            else
            {
                schemeDataRows.ItemsSource = new List<string>() { "Данные не найдены." };
            }
        }

        private Grid CreateGridForDataRow(Row row)
        {
            Grid grid = new();

            List<TextBox> rowColumns = new();
            foreach(var pair in row.Data)
            {
                TextBox textBox = new();
                textBox.Text = pair.Value.ToString();
                textBox.IsReadOnly = true;

                if(pair.Key.ForeignKey != null)
                {
                    ToolTip tip = new();
                    tip.Content = GetToolTip(pair.Key.ForeignKey, textBox.Text);
                    textBox.ToolTip = tip;
                }

                rowColumns.Add(textBox);
            }

            for(int i = 0; i < rowColumns.Count; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.Children.Add(rowColumns[i]);
                Grid.SetColumn(rowColumns[i], i);
            }

            return grid;
        }

        private static double GetMaxWidthInGridColumn(ItemCollection grids, int index)
        {
            double maxWidth = 0;
            
            foreach(Grid grid in grids)
            {
                maxWidth = Math.Max(maxWidth, ((TextBox)grid.Children[index]).Text.Length * 10);
            }

            return maxWidth;
        }

        private string GetToolTip(ForeignKey foreignKey, string value)
        {
            string schemePath = WorkWithFiles.GetFilePath("schemes", foreignKey.Scheme.Name);
            Scheme foreignScheme = WorkWithScheme.ReadScheme(schemePath);

            string dataName = WorkWithFiles.GetSchemeDataName(foreignScheme.Name);
            string dataPath = WorkWithFiles.GetFilePath("data", dataName);
            SchemeData foreignSchemeData = new SchemeData(foreignScheme, dataPath);

            string result = "";
            foreach (Row row in foreignSchemeData.Rows)
            {
                foreach(var pair in row.Data)
                {
                    if(pair.Key.Name == foreignKey.SchemeColumn.Name)
                    {
                        if (value == pair.Value.ToString())
                        {
                            return row.ToString();
                        }
                    }
                }
            }

            return result;
        }

        private void SchemeDataRowsMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                dataScroller.LineUp();
            }
            else
            {
                dataScroller.LineDown();
            }
        }

        private void SchemeColumnsListPreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

        private void SchemeListPreviewMouseWheel(object sender, MouseWheelEventArgs e)
            {
            if (e.Delta > 0)
            {
                schemesScroller.LineUp();
            }
            else
            {
                schemesScroller.LineDown();
            }
        }

        private void OpenWindowForDBCreate(object sender, RoutedEventArgs e)
        {
            CreatingNewDB window = new();
            window.Show();
            window.Owner = this;
        }

        private void OpenWindowForColumnEditing(object sender, RoutedEventArgs e)
        {
            TablesEiditing window = new();
            window.Show();
            window.Owner = this;
        }

        private void OpenWindowForDataEditing(object sender, RoutedEventArgs e)
        {
            DataEditing window = new();
            window.Show();
            window.Owner = this;
        }
    }
}