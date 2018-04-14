using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Crawler.Model
{
    public class Criteria
    {
        [JsonIgnore]
        public virtual Category Category { get; set; }
        [Key]
        public long Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Location Location { get; set; }
        public string Name { get; set; }
        public string ExpectedValue { get; set; }
        public string Selector { get; set; }
        public override bool Equals(object obj)
        {
            var otherType = typeof(object);
            if (obj != null)
                if (obj.GetType().BaseType != null && obj.GetType().Namespace == "System.Data.Entity.DynamicProxies")
                    otherType = obj.GetType().BaseType;
                else
                    otherType = obj.GetType();
            if (obj == null || typeof(Criteria) != otherType)
                return false;
            var other = (Criteria)obj;
            if (!Name.Equals(other.Name))
                return false;
            if (ExpectedValue == null)
            {
                if (other.ExpectedValue != null)
                    return false;
            }
            else
            if (!ExpectedValue.Equals(other.ExpectedValue))
                return false;
            if (Location != other.Location)
                return false;
            if (!Selector.Equals(other.Selector))
                return false;
            return true;

        }
        public override int GetHashCode()
        {
            return Selector.GetHashCode() + ExpectedValue.GetHashCode();
        }
    }

}
