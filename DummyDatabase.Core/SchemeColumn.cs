using System.Text.Json.Serialization;

namespace DummyDatabase.Core
{
    public class SchemeColumn
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; init; }

        public ForeignKey? ForeignKey { get; init; } = null;

        public SchemeColumn(string name, string type, bool isPrimary)
        {
            Name = name;
            Type = type;
            IsPrimary = isPrimary;
        }

        public SchemeColumn()
        { 
        }
    }
}