namespace DB.CodeTemplate
{
    public class Column
    {
        public string ClrType { get; set; }
        public string GeneratedName { get; set; }
        public bool IsComputed { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsKey { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
        public int Length { get; set; }
        public string Name { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public string Type { get; set; }
    }
}
