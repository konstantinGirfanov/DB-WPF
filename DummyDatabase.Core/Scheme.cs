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
            foreach (SchemeColumn column in Columns)
            {
                if(column.ForeignKey != null)
                {
                    columns.Add($"{column.Name} - {column.Type} - Primary: {column.IsPrimary}" +
                    $"; ForeignKey: {column.ForeignKey.Scheme.Name}.{column.ForeignKey.SchemeColumn.Name}");
                }
                else
                {
                    columns.Add($"{column.Name} - {column.Type} - Primary: {column.IsPrimary}" +
                    $"; ForeignKey: null");
                }
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

        public int FindSchemeColumnIndex(SchemeColumn column)
        {
            int number = 0;
            foreach (SchemeColumn thisColumn in Columns)
            {
                if (thisColumn.Name == column.Name)
                {
                    return number;
                }
                else
                {
                    number++;
                }
            }

            return number;
        }
    }
}