using Abot.Poco;
using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Business.Crawling
{
    public interface ICrawlerAgent
    {
        void Init(ApplicationDbContext context, ICrawlerManager manager, Site site);
        void Start();
        void Stop();
        string GetId();
        bool IsRunning();
        string GetBaseUrl();
        event Func<Site,Page,string, CrawledPage,Task> PageCrawled;
    }
    [Serializable]
    public class AgetNotInitializedException : Exception {
        public AgetNotInitializedException() : base() { }
        public AgetNotInitializedException(string message) : base(message) { }
    }
}
