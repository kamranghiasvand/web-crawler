using System;
using Crawler.Business.Crawling;
using Crawler.Model;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Abot.Poco;

namespace Crawler.Business.Matching
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
            log.Info($"Matcher with id {guid} created");
        }
        public string GetId() => guid;

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

        private Task Agent_PageCrawledAsync(Site site, Page page, string text,CrawledPage crawledPage)
        {
            if (!init)
                throw new MatcherNotInitializedException();
            if (site == null)
                throw new ArgumentNullException(nameof(site));
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            if (crawledPage == null)
                throw new ArgumentNullException(nameof(crawledPage));
            return Task.Run(() =>
            {
                log.Debug($"Trying to find Match from {page.Address}");
                if (string.IsNullOrEmpty(text))
                {
                    log.Warn("Page content is empty");
                    return;
                }              
             
                if (site.Categories.Count <= 0)
                {
                    log.Warn("Site definition has no category");
                    return;
                }
                foreach (var cat in site.Categories)
                {
                    log.Debug($"Trying check category with id {cat.Id}");
                 
                    if (cat.Filters.Count <= 0)
                    {
                        AddIdToCategoriesList(page, cat.Id);
                        log.Warn($"Category with id {cat.Id} definition has no filter");
                        continue;
                    }
                    #region Check Filter
                    var record = CheckFilters(crawledPage,cat);
                    #endregion
                    AddIdToCategoriesList(page, cat.Id);
                    LogRecord(record);
                    CallRecordFound(cat, record);
                    log.Debug($"Done check category with id {cat.Id}");
                }
            });
        }
        private static Dictionary<string,string> CheckFilters(CrawledPage page , Category cat)
        {            
            var record = new Dictionary<string, string>();
            foreach (var filter in cat.Filters)
            {
                try
                {
                    log.Debug($"Trying filter with id {filter.Id}");
                    if (string.IsNullOrEmpty(filter.XPath))
                    {
                        log.Warn("Filter has no XPath");
                        continue;
                    }
                    var node = page.AngleSharpHtmlDocument.QuerySelector(filter.XPath);
                    if (node != null)
                    {
                        log.Debug("node found");
                        if (filter.Location == Location.Attribute)
                        {
                            var attr = node.Attributes[filter.Name];
                            log.Debug($"Found value  [{attr.Value}] for filter with id {filter.Id}");
                            record.Add(filter.OutName, attr.Value);
                        }
                        else if (filter.Location == Location.InnerText)
                        {
                            log.Debug($"Found value  [{node.InnerHtml}]  for filter with id {filter.Id}");
                            record.Add(filter.OutName, node.InnerHtml);
                        }
                    }
                    else
                    {
                        log.Debug($"node not found");
                    }
                }
                catch(Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
            }
            return record;
        }
        private void AddIdToCategoriesList(Page page, long catId)
        {
            if (page == null)
                return;
            if (page.CategoriesId == null)
                page.CategoriesId = new List<long>();
            page.CategoriesId.Add(catId);
            context.Entry(page).State = System.Data.Entity.EntityState.Modified;
        }
        private static void LogRecord(Dictionary<string, string> record)
        {
            if (record == null)
            {
                log.Warn("Record is null");
                return;
            }
            if(record.Count==0)
            {
                log.Info("Record not found in page");
                return;
            }            
            log.Debug($"record has {record.Count} element");
            var str = "Record found in page: [";
            var builder = new StringBuilder();
            builder.Append(str);
            foreach (var item in record)
                builder.Append("{" + item.Key + "," + item.Value + "},");
            str = builder.ToString();
            str = str.Substring(0, str.Length - 1) + "]";
            log.Info(str);
        }
        private void CallRecordFound(Category cat, Dictionary<string, string> record)
        {
            if (record == null || record.Count == 0)
                return;
            log.Debug("Calling Manager MatchFound");
            (new Thread(() => { manager?.MatchFound(this, cat, record); })).Start();

        }
    }
}
