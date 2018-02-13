using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext():base("DefaultConnection")
        {
        }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Filter> Filters { get; set; }
    }
}
