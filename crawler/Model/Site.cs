using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.Model
{
    public class Site
    {
        [Key]
        public long Id { get; set; }
        public string BaseUrl { get; set; }
        public virtual ICollection<Verification> Verifications { get; set; } = new List<Verification>();
        public virtual ICollection<Page> AvaileblePages { get; set; } = new List<Page>();
        public virtual ICollection<string> SeenPages { get; set; } = new List<string>();


    }
}
