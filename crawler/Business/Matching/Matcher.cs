using System;
using crawler.Business.Crawling;
using crawler.Model;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace crawler.Business.Matching
{
    public class Matcher : IMatcher
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Matcher));
        private ApplicationDbContext context;
        private ICrawlerAgent agent;
        private IMatcherManager manager;
        private bool init;
        private readonly string guid;
        public Matcher()
        {
            guid = Guid.NewGuid().ToString();
        }
        public string GetId() { return guid; }
        public void Init(ApplicationDbContext context, ICrawlerAgent agent, IMatcherManager manager)
        {
            log.Debug("Initializing Matcher");
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (agent == null)
                throw new ArgumentNullException(nameof(agent));
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            this.context = context;
            this.agent = agent;
            this.manager = manager;
            this.agent.PageCrawled += Agent_PageCrawledAsync;
            log.Debug($"Attached event listener to PageCrawled of agent with id {this.agent.GetId()}");
            init = true;
        }

        private Task Agent_PageCrawledAsync(Site site, Page page, string text)
        {
            if (!init)
                throw new MatcherNotInitializedException();
            if (site == null)
                throw new ArgumentNullException(nameof(site));
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            return Task.Run(() =>
            {
                log.Debug($"Trying to find Match from {page.Address}");
                if (string.IsNullOrEmpty(text))
                {
                    log.Warn("Page content is empty");
                    return;
                }
                context.Sites.Attach(site);
                var html = new HtmlDocument();
                html.LoadHtml(text);
                if(site.Categories.Count<=0)
                {
                    log.Warn("Site definition has no category");
                    return;
                }
                foreach (var cat in site.Categories)
                {
                    log.Debug($"Trying check category with id {cat.Id}");
                    var record = new Dictionary<string, string>();
                    if(cat.Filters.Count<=0)
                    {
                        log.Warn($"Category with id {cat.Id} definition has no filter");
                        continue;
                    }
                    foreach (var filter in cat.Filters)
                    {
                        log.Debug($"Trying filter with id{filter.Id}");
                        var node = html.DocumentNode.SelectSingleNode(filter.XPath);
                        if (node != null)
                        {
                            log.Debug("node found");
                            if (filter.Location == Location.Attribute)
                            {
                                var attr = node.Attributes[filter.Name];
                                record.Add(filter.OutName, attr.Value);
                            }
                            else if (filter.Location == Location.InnerText)
                            {
                                record.Add(filter.OutName, node.InnerText);
                            }
                        }
                        else
                        {
                            log.Debug($"node not found");
                        }
                    }
                    log.Debug($"record has {record.Count} element");


                    log.Debug($"Done check category with id {cat.Id}");
                }
            });
        }
    }
}
