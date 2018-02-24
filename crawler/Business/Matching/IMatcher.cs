using Crawler.Business.Crawling;
using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Business.Matching
{
    public interface IMatcher
    {
        void Init(ApplicationDbContext context, ICrawlerAgent agent,IMatcherManager manager);
        string GetId();
    }
    [Serializable]
    public class MatcherNotInitializedException : Exception
    {
        public MatcherNotInitializedException() : base() { }
        public MatcherNotInitializedException(string message) : base(message) { }
    }
}
