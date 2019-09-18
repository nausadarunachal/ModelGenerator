namespace DB.CodeTemplate
{
    using System.Collections.Generic;

    public class ForeignKey
    {
        public List<ForeignKeyColumn> Columns { get; set; } 
            = new List<ForeignKeyColumn>();
        public string DestinationTableName { get; set; }
        public string SourceTableName { get; set; }
    }
}
