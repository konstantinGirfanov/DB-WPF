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
        
        [JsonPropertyName("foreignKey")]
        public ForeignKey? ForeignKey { get; init; }

        public SchemeColumn(string name, string type, bool isPrimary, ForeignKey foreignKey)
        {
            Name = name;
            Type = type;
            IsPrimary = isPrimary;
            ForeignKey = foreignKey;
        }

        public SchemeColumn(string name, string type, bool isPrimary)
        {
            Name = name;
            Type = type;
            IsPrimary = isPrimary;
            ForeignKey = null;
        }

        public SchemeColumn()
        {  }
    }
}