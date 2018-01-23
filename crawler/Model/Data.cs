using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.Model
{
    [Table("Data")]
    public class Data
    {
        [Key]
        public long Id { get; set; }
        public String Url { get; set; }
        public String Text { get; set; }
    }
}
