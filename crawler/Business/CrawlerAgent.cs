using Abot.Crawler;
using crawler.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.Business
{
    public class CrawlerAgent:ICrawlerAgent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CrawlerAgent));
        private readonly DbContext context;
        private Site site;
        private string guid;
        private PoliteWebCrawler agent;
        public CrawlerAgent(DbContext context, Site site)
        {
            this.context = context;
            this.site = site;
            guid = Guid.NewGuid().ToString();
        }

        public string GetId()
        {
            return guid;
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
