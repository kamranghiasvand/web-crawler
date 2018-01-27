using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace crawler.Model
{
    [Table(nameof(Page))]
    public class Page
    {
        [Key]
        public long Id { get; set; }
        public string Address { get; set; }
        public string Text { get; set; }
        public DateTime SeeTime { get; set; }                
    }    
}
