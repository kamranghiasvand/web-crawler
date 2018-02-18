using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Crawler.Model
{
    public class Site
    {
        [Key]
        public long Id { get; set; }
        public string BaseUrl { get; set; }
        public virtual ICollection<Page> Pages { get; set; } = new List<Page>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public string OutputFolder { get; set; }
    }
}
