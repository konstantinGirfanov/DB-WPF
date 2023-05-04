using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using DummyDatabase.Core;

namespace DummyDatabase.Desktop.windows_for_editing.columns
{
    /// <summary>
    /// Логика взаимодействия для DataEditing.xaml
    /// </summary>
    public partial class DataEditing : Window
    {
        Scheme scheme;
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
            scheme = WorkWithScheme.ReadScheme($"{schemesPath}\\{schemeName}");

            string schemeDataName = WorkWithFiles.GetSchemeDataName(scheme.Name);
            string dataFolderPath = WorkWithFiles.GetFolderPath("data");

            dataTree.Items.Clear();
            if (File.Exists($"{dataFolderPath}\\{schemeDataName}"))
            {
                List<Row> dataList = new SchemeData(scheme, $"{dataFolderPath}\\{schemeDataName}").Rows;
                
                int count = 0;
                foreach(Row row in dataList)
                {
                    dataTree.Items.Add(CreateDataRow(row, count));
                    count++;
                }
            }
        }

        private void ScrollSchemeListScroller(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
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
            if(e.Delta > 0)
            {
                dataScroller.LineUp();
            }
            else
            {
                dataScroller.LineDown();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private static TreeViewItem CreateDataRow(Row dataRow, int rowNumber)
        {
            TreeViewItem row = new();
            row.Header = $"{rowNumber} строка";

            foreach(KeyValuePair<SchemeColumn, object> pair in dataRow.Data)
            {
                Grid rowGrid = new();
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock rowColumnInfo = new();
                rowColumnInfo.Text = $"{pair.Key.Name} - {pair.Key.Type}:";
                rowGrid.Children.Add(rowColumnInfo);
                Grid.SetColumn(rowColumnInfo, 0);

                TextBox rowColumnValue = new();
                rowColumnValue.Text = pair.Value.ToString();
                rowGrid.Children.Add(rowColumnValue);
                Grid.SetColumn(rowColumnValue, 1);

                row.Items.Add(rowGrid);
            }

            return row;
        }


    }
}
