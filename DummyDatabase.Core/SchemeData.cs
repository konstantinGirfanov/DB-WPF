namespace DummyDatabase.Core
{
    public class SchemeData
    {
        public List<Row> Rows { get; set; }
        public Scheme Scheme { get; init; }

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
}