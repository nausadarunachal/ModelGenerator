namespace DB.CodeTemplate
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class NavigationProperty
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Multiplicity DestinationMultiplicity { get; set; }

        public ForeignKey ForeignKey { get; set; }

        public string InverseProperty { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Multiplicity SourceMultiplicity { get; set; }

        public string Type { get; set; }
    }
}
