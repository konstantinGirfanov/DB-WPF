using System.Text.Json;

namespace DummyDatabase.Core
{
    public class ForeignKey
    {
        public Scheme Scheme { get; set; }

        public SchemeColumn SchemeColumn { get; set; }

        public ForeignKey(string schemePath, string schemeColumnName)
        {
            Scheme = WorkWithScheme.ReadScheme(schemePath);
            SchemeColumn = Scheme.FindSchemeColumn(schemeColumnName);
        }
    }
}
