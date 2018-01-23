using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace crawler.Model
{
    [Table(nameof(Data))]
    public class Data
    {
        [Key]
        public long Id { get; set; }
        public string Url { get; set; }
        public string Text { get; set; }
        public DateTime CrawledTime { get; set; }
                
    }
    public enum DataState
    {
        FaildToRetrive=0,
        EmptyContent=1,
        NotRelative=2,

    }
}
