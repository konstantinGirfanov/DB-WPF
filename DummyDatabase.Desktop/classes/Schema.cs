using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBCore
{
    class Scheme
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
                columns.Add(column.Name + " - " + column.Type);
            }

            return columns;
        }
    }

    class SchemeColumn
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; init; }

        public SchemeColumn(string name, string type, bool isPrimary)
        {
            Name = name;
            Type = type;
            IsPrimary = isPrimary;
        }
    }

    class SchemeData
    {
        public List<Row> Rows { get; set; }
        private Scheme Scheme { get; init; }

        public SchemeData(Scheme scheme, string path)
        {
            Scheme = scheme;
            Rows = GetData(path);
        }

        public List<Row> GetData(string path)
        {
            string[] data = File.ReadAllLines(path);
            List<Row> rows = new();

            for (int i = 1; i < data.Length; i++)
            {
                if (WorkWithScheme.IsCorrespondsToScheme(Scheme, data[i], i))
                {
                    rows.Add(new Row(Scheme, data[i]));
                }
            }

            return rows;
        }
    }

    class Row
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
                sb.Append(columnValue + "   ");
            }

            return sb.ToString();
        }
    }

    static class WorkWithScheme
    {
        public static Scheme ReadScheme(string path)
        {
            return JsonSerializer.Deserialize<Scheme>(File.ReadAllText(path));
        }

        public static bool IsCorrespondsToScheme(Scheme scheme, string line, int row)
        {
            string[] lineColumns = line.Split(';');

            if (scheme.Columns.Length != lineColumns.Length)
            {
                DisplayErrorMessage(false, row + 1, 0);
                return false;
            }
            else
            {
                bool isCorresponded = true;
                for (int i = 0; i < lineColumns.Length; i++)
                {
                    switch (scheme.Columns[i].Type)
                    {
                        case "int":
                            if (!int.TryParse(lineColumns[i], out int _))
                            {
                                DisplayErrorMessage(true, row + 1, i + 1);
                                isCorresponded = false;
                            }
                            break;
                        case "float":
                            if (!float.TryParse(lineColumns[i], out float _))
                            {
                                DisplayErrorMessage(true, row + 1, i + 1);
                                isCorresponded = false;
                            }
                            break;
                        case "double":
                            if (!double.TryParse(lineColumns[i], out double _))
                            {
                                DisplayErrorMessage(true, row + 1, i + 1);
                                isCorresponded = false;
                            }
                            break;
                        case "bool":
                            if (!bool.TryParse(lineColumns[i], out bool _))
                            {
                                DisplayErrorMessage(true, row + 1, i + 1);
                                isCorresponded = false;
                            }
                            break;
                        case "dateTime":
                            if (!DateTime.TryParse(lineColumns[i], out DateTime _))
                            {
                                DisplayErrorMessage(true, row + 1, i + 1);
                                isCorresponded = false;
                            }
                            break;
                    }
                }

                return isCorresponded;
            }
        }

        public static void DisplayErrorMessage(bool isCorrectColumnCount, int row, int column)
        {
            if (isCorrectColumnCount)
            {
                //ничего
            }
            else
            {
                //или ничего?
            }
        }
    }
}