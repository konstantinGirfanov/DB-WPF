using System.Text;

namespace DummyDatabase.Core
{
    public class Row
    {
        public Dictionary<SchemeColumn, object> Data { get; set; } = new();

        public Row(Scheme scheme, string line)
        {
            string[] columnValues = line.Split(';');

            for (int i = 0; i < columnValues.Length; i++)
            {
                Data.Add(scheme.Columns[i], columnValues[i]);
            }
        }

        private Row()
        {
            Data = new Dictionary<SchemeColumn, object>();
        }

        public static Row CreateEmptyRowData(Scheme scheme)
        {
            Row row = new();

            foreach(SchemeColumn column in scheme.Columns)
            {
                row.Data.Add(column, "");
            }

            return row;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            foreach (string columnValue in Data.Values.Cast<string>())
            {
                sb.Append($"{columnValue}   ");
            }

            return sb.ToString();
        }

        public string FindValue(SchemeColumn column)
        {
            foreach(KeyValuePair<SchemeColumn, object> pair in Data)
            {
                if(pair.Key.Name == column.Name)
                {
                    return pair.Value.ToString();
                }
            }

            return "";
        }
    }
}