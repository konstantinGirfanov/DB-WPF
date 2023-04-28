using System.Text;

namespace DummyDatabase.Core
{
    public class Row
    {
        private Dictionary<SchemeColumn, object> Data { get; set; } = new Dictionary<SchemeColumn, object>();

        public Row(Scheme scheme, string line)
        {
            string[] columnValues = line.Split(';');

            for (int i = 0; i < columnValues.Length; i++)
            {
                Data.Add(scheme.Columns[i], columnValues[i]);
            }
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
    }
}