using System.Text.Json.Serialization;

namespace DummyDatabase.Core
{
    public class ForeignKey
    {
        [JsonPropertyName("scheme")]
        public Scheme Scheme { get; init; }

        [JsonPropertyName("schemeColumn")]
        public SchemeColumn SchemeColumn { get; init; }

        public ForeignKey(string schemePath, string schemeColumnName)
        {
            Scheme = WorkWithScheme.ReadScheme(schemePath);
            SchemeColumn = Scheme.FindSchemeColumn(schemeColumnName);
        }

        public ForeignKey()
        { }
    }
}
