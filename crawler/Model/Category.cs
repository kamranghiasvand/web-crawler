using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crawler.Model
{
    public class Category
    {
        [Key]
        public long Id { get; set; }
        [Index(IsUnique =true)]
        [Column(TypeName = "VARCHAR")]
        [StringLength(400)]
        public string Name { get; set; }
        public virtual ICollection<Filter> Filters { get; set; } = new List<Filter>();
        public virtual Site site { get; set; }
    }
}