using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using DummyDatabase.Core;

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
            var schemesPath = WorkWithFiles.GetFolderPath("schemes");
            string schemeName = schemeList.SelectedItem.ToString();
            scheme = WorkWithScheme.ReadScheme(schemesPath + $"\\{schemeName}");

            schemeColumnsList.ItemsSource = scheme.GetSchemeColumns();
        }

        private void LoadData()
        {
            string schemeDataName = WorkWithFiles.GetSchemeDataName(scheme.Name);
            string dataFolderPath = WorkWithFiles.GetFolderPath("data");
            if (File.Exists(dataFolderPath + $"\\{schemeDataName}"))
            {
                schemeDataRows.ItemsSource = new SchemeData(scheme, dataFolderPath + $"\\{schemeDataName}").Rows;
            }
            else
            {
                schemeDataRows.ItemsSource = new List<string>() { "Данные не найдены." };
            }
        }
    }
}
