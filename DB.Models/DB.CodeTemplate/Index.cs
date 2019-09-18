namespace DB.CodeTemplate
{
    using System.Collections.Generic;

    public class Index
    {
        public List<string> Columns { get; } = new List<string>();
        public int IndexId { get; set; }
        public string Name { get; set; }
    }
}
