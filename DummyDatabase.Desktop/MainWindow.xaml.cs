using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBCore;

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

        }
    }
}
