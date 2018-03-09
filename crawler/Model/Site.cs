using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Crawler.Model
{
    public class Site
    {
        public string BaseUrl { get; set; }
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string OutputFolder { get; set; }
        [JsonIgnore]
        public virtual ICollection<Page> Pages { get; set; } = new List<Page>();
        public override bool Equals(object obj)
        {
            var otherType = typeof(object);
            if (obj != null)
                if (obj.GetType().BaseType != null && obj.GetType().Namespace == "System.Data.Entity.DynamicProxies")
                    otherType = obj.GetType().BaseType;
                else
                    otherType = obj.GetType();
            if (obj == null || otherType != typeof(Site))
                return false;
            var other = (Site)obj;
            var otherUri = new Uri(other.BaseUrl);
            var uri = new Uri(BaseUrl);
            return Uri.Compare(uri, otherUri, UriComponents.HostAndPort, UriFormat.UriEscaped, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
        public override int GetHashCode()
        {
            return BaseUrl.GetHashCode();
        }
    }
}
