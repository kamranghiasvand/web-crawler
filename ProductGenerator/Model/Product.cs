using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductGenerator.Model
{
    public class Product
    {
        public string Name { get; set; }
        public Dictionary<string, Feature> Features { get; set; } = new Dictionary<string, Feature>();
    }
}
