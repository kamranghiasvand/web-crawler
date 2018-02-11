using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace crawler.Model
{
    public class Category
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Filter> Filters { get; set; }
    }
}