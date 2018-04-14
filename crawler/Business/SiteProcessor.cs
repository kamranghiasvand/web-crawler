using Crawler.Business.Crawling;
using Crawler.Business.Matching;
using Crawler.Business.Storing;
using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Business
{
    public class SiteProcessor : ICrawlerManager, IMatcherManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SiteProcessor));
        private readonly ApplicationDbContext context;
        private Site site;
        private ICrawlerAgent crawler;
        private IMatcher matcher;
        private Dictionary<string, IStoreAgent> storeAgents = new Dictionary<string, IStoreAgent>();
        private bool isRunning;
        public SiteProcessor(ApplicationDbContext context, Site site)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (site == null)
                throw new ArgumentNullException(nameof(site));
            this.context = context;
            this.site = site;
        }
        public string GetBaseUrl
        {
            get { return site.BaseUrl; }
        }
        public void Start()
        {
            log.Debug("Site Processor starting...");
            if (isRunning)
            {
                log.Warn("Site Processor is already running");
                return;
            }
            isRunning = true;
            crawler = new CrawlerAgent();
            crawler.Init(this, site.Id);

            matcher = new Matcher();
            matcher.Init( crawler, this);
            foreach (var cat in site.Categories)
            {
                var agent = new StoreAgent();
                agent.Init(cat.Id, site.OutputFolder);
                storeAgents.Add(cat.Name, agent);
            }
            crawler.Start();
            log.Debug("Site Processor started");
        }
        public void Stop()
        {
            if (!isRunning)
            {
                log.Warn("Site Processor is already stopped");
                return;
            }
            isRunning = false;
            crawler.Stop();
        }

        #region Crawler Manager
        public void Done(ICrawlerAgent agent)
        {
            context.SaveChanges();
            foreach (var s in storeAgents.Values)
                s.Close();
            isRunning = false;
        }
        #endregion

        #region Matcher Manager
        public void MatchFound(IMatcher matcher, Category category, Dictionary<string, string> record)
        {
            IStoreAgent store = null;
            if (!storeAgents.TryGetValue(category.Name, out store))
            {
                log.Warn($"Store agent not found for category {category.Name}");
                return;
            }
            store.Store(record);
        }
        #endregion

    }
}
