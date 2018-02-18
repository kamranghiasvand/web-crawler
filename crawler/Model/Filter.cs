using System.ComponentModel.DataAnnotations;

namespace Crawler.Model
{
    public class Filter
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string OutName { get; set; }
        public string XPath { get; set; }
        public ValueType Type { get; set; }
        public Location Location { get; set; }
        public virtual Category Category { get; set; }
    }
    public enum Location { Attribute,InnerText}
    public enum ValueType
    {
        txt,
        png,
        jpg
    }
}
