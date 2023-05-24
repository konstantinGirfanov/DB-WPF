﻿using System.Collections.Generic;
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
                schemeDataRows.ItemsSource = new SchemeData(scheme, $"{dataFolderPath}\\{schemeDataName}").Rows;
            }
            else
            {
                schemeDataRows.ItemsSource = new List<string>() { "Данные не найдены." };
            }
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
