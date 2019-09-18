namespace DB.CodeTemplate
{
    using System.Collections.Generic;

    public class Table
    {
        public string ActiveColumnName { get; set; }
        public bool ActiveColumnValue { get; set; }
        public List<Column> Columns { get; set; }
            = new List<Column>();
        public List<ForeignKey> ForeignKeys { get; set; }
            = new List<ForeignKey>();
        public string GeneratedName { get; set; }
        public List<Index> Indexes { get; set; }
            = new List<Index>();
        public List<NavigationProperty> NavigationProperties { get; }
            = new List<NavigationProperty>();
        public int ObjectId { get; set; }
        public List<string> PrimaryKeyColumns { get; set; }
            = new List<string>();
        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}
