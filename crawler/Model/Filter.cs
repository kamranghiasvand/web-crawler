using System.ComponentModel.DataAnnotations;

namespace crawler.Model
{
    public class Filter
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string XPath { get; set; }
        public ValueType Type { get; set; }
        public virtual Category Category { get; set; }
    }
    public enum ValueType
    {
        txt,
        png,
        jpg
    }
}
