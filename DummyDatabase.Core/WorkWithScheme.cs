﻿using System.Text.Json;

namespace DummyDatabase.Core
{
    public static class WorkWithScheme
    {
        public static Scheme ReadScheme(string path)
        {
            return JsonSerializer.Deserialize<Scheme>(File.ReadAllText(path));
        }

        public static bool IsAbleToAdd(Scheme scheme, List<Row> rows, string line)
        {
            return IsCorrespondsToScheme(scheme, line) & IsNotContained(line, rows, scheme);
        }

        public static bool IsCorrespondsToScheme(Scheme scheme, string line)
        {
            string[] lineColumns = line.Split(';');

            if (scheme.Columns.Length != lineColumns.Length)
            {
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
                                isCorresponded = false;
                            }
                            break;
                        case "float":
                            if (!float.TryParse(lineColumns[i], out float _))
                            {
                                isCorresponded = false;
                            }
                            break;
                        case "double":
                            if (!double.TryParse(lineColumns[i], out double _))
                            {
                                isCorresponded = false;
                            }
                            break;
                        case "bool":
                            if (!bool.TryParse(lineColumns[i], out bool _))
                            {
                                isCorresponded = false;
                            }
                            break;
                        case "dateTime":
                            if (!DateTime.TryParse(lineColumns[i], out DateTime _))
                            {
                                isCorresponded = false;
                            }
                            break;
                    }
                }

                return isCorresponded;
            }
        }

        public static bool IsNotContained(string line, List<Row> rows, Scheme scheme)
        {
            bool isNotContained = true;
            int numberPrimaryColumn = 0;
            for(int i = 0; i < scheme.Columns.Length; i++)
            {
                if (scheme.Columns[i].IsPrimary)
                {
                    numberPrimaryColumn = i;
                    break;
                }
            }

            string linePrimaryValue = line.Split(";")[numberPrimaryColumn];

            foreach(Row row in rows)
            {
                if(linePrimaryValue == row.ToString().Split("   ")[numberPrimaryColumn])
                {
                    isNotContained = false;
                }
            }

            return isNotContained;
        }
    }
}