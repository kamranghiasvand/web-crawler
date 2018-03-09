using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Crawler.Model
{
    public class Category
    {
        public virtual ICollection<Criteria> Criteria { get; set; } = new List<Criteria>();
        public virtual ICollection<Filter> Filters { get; set; } = new List<Filter>();
        [Key]
        public long Id { get; set; }
        [Index(IsUnique = true)]
        [Column(TypeName = "VARCHAR")]
        [StringLength(400)]
        public string Name { get; set; }
        [JsonIgnore]
        public virtual Site site { get; set; }
        public override bool Equals(object obj)
        {
            var otherType = typeof(object);
            if (obj != null)
                if (obj.GetType().BaseType != null && obj.GetType().Namespace == "System.Data.Entity.DynamicProxies")
                    otherType = obj.GetType().BaseType;
                else
                    otherType = obj.GetType();
            if (obj == null || typeof(Category) != otherType)
                return false;
            var other = (Category)obj;
            if (Filters.Count != other.Filters.Count)
                return false;
            if (Criteria.Count != other.Criteria.Count)
                return false;
            foreach (var filter in Filters)
                if (other.Filters.FirstOrDefault(m => m.Equals(filter)) == null)
                    return false;
            foreach (var filter in Criteria)
                if (other.Criteria.FirstOrDefault(m => m.Equals(filter)) == null)
                    return false;
            if (!Name.Equals(other.Name))
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode()+Name.GetHashCode();
        }
    }
}