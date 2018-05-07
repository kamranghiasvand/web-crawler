using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductGenerator.Model
{
    public class Feature
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool Multiple { get; set; }
        public string Default { get; set; }
        public List<FeatureValue> Values { get; set; } = new List<FeatureValue>();
    }
}
