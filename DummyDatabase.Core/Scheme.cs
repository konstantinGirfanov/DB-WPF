using System.Text.Json.Serialization;

namespace DummyDatabase.Core
{
    public class Scheme
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("columns")]
        public SchemeColumn[] Columns { get; init; }

        public Scheme(string name, SchemeColumn[] columns)
        {
            Name = name;
            Columns = columns;
        }

        public Scheme()
        {
        }

        public List<string> GetSchemeColumns()
        {
            List<string> columns = new();
            columns.Add("Список столбцов таблицы:");
            foreach (SchemeColumn column in Columns)
            {
                columns.Add($"{column.Name} - {column.Type} - {column.IsPrimary}");
            }

            return columns;
        }
    }
}