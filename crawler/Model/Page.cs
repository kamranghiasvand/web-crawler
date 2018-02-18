using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crawler.Model
{
    [Table(nameof(Page))]
    public class Page
    {
        [Key]
        public long Id { get; set; }
        public string Address { get; set; }
        public string Text { get; set; }
        public DateTime SeeTime { get; set; }
        public bool IsSuccess { get; set; }
        public List<long> CategoriesId { get; set; }
    }
}
