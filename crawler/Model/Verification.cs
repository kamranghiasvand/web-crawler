using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.Model
{
    public class Verification
    {
        [Key]
        public long Id { get; set; }
        public string Xpath { get; set; }
        public string Value { get; set; }
    }

}
