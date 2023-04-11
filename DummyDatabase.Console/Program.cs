using System.Text.Json.Serialization;

namespace DBConsole
{
    class Program
    {
        public static void Main()
        {

        }
    }

    class Scheme
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("columns")]
        public SchemeColumn[] Columns { get; init; }
    }

    class SchemeColumn
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; init; }
    }

    class SchemeData
    {
        public List<Row> Rows { get; set; }
        private Scheme Scheme { get; init; }

        public void PrintData()
        {
            Console.WriteLine($"Имя базы данных: {Scheme.Name}.");
            Console.WriteLine("Названия столбцов:");
            for (int i = 0; i < Scheme.Columns.Length; i++)
            {
                Console.Write(Scheme.Columns[i].Name + "  ");
            }
            Console.WriteLine("");
            foreach (Row row in Rows)
            {
                row.PrintRow();
            }
        }
    }

    class Row
    {
        private Dictionary<SchemeColumn, object> Data { get; set; } = new Dictionary<SchemeColumn, object>();

        public void PrintRow()
        {
            foreach (KeyValuePair<SchemeColumn, object> column in Data)
            {
                Console.Write(column.Value + "  ");
            }

            Console.WriteLine();
        }
    }
}