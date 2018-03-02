using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Crawler.Model
{
    public class Filter
    {
        [JsonIgnore]
        public virtual Category Category { get; set; }
        [Key]
        public long Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Location Location { get; set; }
        public string Name { get; set; }
        public string OutName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueType Type { get; set; }
        public string Selector { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            var other = (Filter)obj;
            if (!Name.Equals(other.Name))
                return false;
            if (Location != other.Location)
                return false;
            if (!Selector.Equals(other.Selector))
                return false;
            return true;

        }
        public override int GetHashCode()
        {
            return Selector.GetHashCode() + Name.GetHashCode();
        }
    }
    public enum Location { Attribute, InnerText }
    public enum ValueType
    {
        txt,
        png,
        jpg
    }
}
