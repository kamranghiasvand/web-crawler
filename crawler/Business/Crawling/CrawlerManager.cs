using Abot.Crawler;
using crawler.Business;
using crawler.Model;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace crawler.Business.Crawling
{
    public class CrawlerManager:ICrawlerManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CrawlerManager));
        private readonly Dictionary<string, ICrawlerAgent> agents = new Dictionary<string, ICrawlerAgent>();
        private ApplicationDbContext context;
        public CrawlerManager()
        {

        }

        public void AddNewSite(Site site)
        {
            log.Debug("Adding New site");
            context.Sites.Attach(site);
            if (site == null)
                throw new ArgumentNullException(nameof(site));
            var agent = agents.Values.FirstOrDefault(m => m.GetBaseUrl() == site.BaseUrl);
            if (agent == null)
            {
                log.Debug("Site not found in current dictionary");
                agent = new CrawlerAgent();
                agent.Init(context, this, site);
                agents.Add(agent.GetId(), agent);

            }
            else
                log.Debug("site found in current dictionary");
            agent.Start();
        }

        public void Done(ICrawlerAgent agent)
        {

        }

        public void Init(ApplicationDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this.context = context;
        }

        public void Start()
        {
            log.Debug("starting CrawlerManager");
            foreach(var item in context.Sites)
            {
                ICrawlerAgent agent = new CrawlerAgent();
                agent.Init(context, this, item);
                agents.Add(agent.GetId(), agent);
                agent.Start();
            }


        }
        public void Stop()
        {
            log.Debug("Stopping CrawlerManager");
            foreach (var item in agents.Values)
                item.Stop();
        }
    }
}
