using System.Text.Json.Serialization;

namespace DummyDatabase.Core
{
    public class Scheme
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("columns")]
        public SchemeColumn[] Columns { get; set; }

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
                columns.Add($"{column.Name} - {column.Type} - Primary: {column.IsPrimary}");
            }

            return columns;
        }

        public SchemeColumn FindSchemeColumn(string columnName)
        {
            foreach(SchemeColumn column in Columns)
            {
                if(column.Name == columnName)
                {
                    return column;
                }
            }

            throw new Exception($"Столбец с именем {columnName} не найден в таблице {Name}");
        }
    }
}