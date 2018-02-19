using Crawler.Business.Crawling;
using Crawler.Business.Matching;
using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Crawler.Business
{
    public class Engine :ICrawlerManager, IMatcherManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Engine));
        private ApplicationDbContext context;
    
        #region ICrawlerManager
        private readonly Dictionary<string, ICrawlerAgent> agents = new Dictionary<string, ICrawlerAgent>();
        public Engine(ApplicationDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this.context = context;
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

        public void Start()
        {
            log.Debug("starting CrawlerManager");
            foreach (var item in context.Sites)
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
        #endregion

        #region IMatcherManager
        public void MatchFound(IMatcher matcher, Category category, Dictionary<string, string> record)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
