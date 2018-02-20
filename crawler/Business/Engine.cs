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
    public class Engine
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Engine));
        private ApplicationDbContext context;
        private List<SiteProcessor> processors = new List<SiteProcessor>();
        private bool isRunning;
        public Engine(ApplicationDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this.context = context;
        }
        public void Start()
        {
            if(isRunning)
            {
                log.Warn("Engine is already started");
                return;
            }
            isRunning = true;
         
                try
                {
                    foreach (var site in context.Sites)
                    {
                        log.Debug($"Creating processor for site {site.Name} || {site.BaseUrl}");
                        var proc = new SiteProcessor(context, site);
                        processors.Add(proc);
                        proc.Start();
                    }
                }
                catch(Exception ex)
                {
                    log.Fatal(ex.Message,ex);
                }
        }
        public void Stop()
        {
            if(!isRunning)
            {
                log.Warn("Engine is already stopped");
                return;
            }
            isRunning = false;
        }

        //public void AddNewSite(Site site)
        //{
        //    if (site == null)
        //        throw new ArgumentNullException(nameof(site));
        //    log.Debug("Adding New site");
        //    if (context.Sites.FirstOrDefault(m => m.BaseUrl == site.BaseUrl) == null)
        //    {
        //        log.Debug("Adding Site to database");
        //        context.Sites.Add(site);
        //        context.SaveChanges();
        //    }
        //    else
        //        context.Sites.Attach(site);

        //    var agent = agents.Values.FirstOrDefault(m => m.GetBaseUrl() == site.BaseUrl);
        //    if (agent == null)
        //    {
        //        log.Debug("no agent for site found in current dictionary. Creating new one");
        //        agent = new CrawlerAgent();
        //        agent.Init(context, this, site);
        //        agents.Add(agent.GetId(), agent);

        //    }
        //    else
        //        log.Debug("agent for site found in current dictionary");

        //    agent.Start();
        //}
       
    }
}
