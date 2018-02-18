using Abot.Crawler;
using Abot.Poco;
using AbotX.Crawler;
using Crawler.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.Business.Crawling
{
    public class CrawlerAgent : ICrawlerAgent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CrawlerAgent));
        private ApplicationDbContext dbContext;
        private Site site;
        private readonly string guid;
        private CrawlerX agent;
        private bool isRunning;
        private ICrawlerManager manager;
        private bool init;

        public event Func<Site, Page,string, Task> PageCrawled;

        public CrawlerAgent()
        {
            guid = Guid.NewGuid().ToString();
            log.Debug($"Crawler agent with id {guid} was created");

        }
        public string GetId()
        {
            if (!init)
                throw new AgetNotInitializedException();
            return guid;
        }
        public string GetBaseUrl()
        {
            if (!init)
                throw new AgetNotInitializedException();
            return site.BaseUrl;
        }
        public void Start()
        {
            if (!init)
                throw new AgetNotInitializedException();
            lock (this)
            {
                log.Debug($"Starting crawler with id {guid}");
                if (isRunning)
                {
                    log.Info($"Crawler with id {guid} is already started");
                    return;
                }
                log.Debug($"Initializing CrawlerX");
                isRunning = true;
                agent = new CrawlerX();
                agent.PageCrawlCompletedAsync += Agent_PageCrawlCompleted;
                agent.PageLinksCrawlDisallowedAsync += Agent_PageLinksCrawlDisallowed;
                agent.ShouldCrawlPage(ShouldCrawlPage);

                (new Thread(() =>
                {
                    log.Debug("Trying to start CrawlX");
                    agent.Crawl(new Uri(site.BaseUrl));
                    log.Info("Crawling is done");
                    lock (this) isRunning = false;
                    log.Debug("Calling manager");
                    manager.Done(this);

                })).Start();
            }

        }
        public void Stop()
        {
            if (!init)
                throw new AgetNotInitializedException();
            lock (this)
            {
                if (!isRunning)
                    return;
                agent.Stop(true);
                isRunning = false;
            }
        }
        public bool IsRunning()
        {
            if (!init)
                throw new AgetNotInitializedException();
            return isRunning;
        }

        public void Init(ApplicationDbContext context, ICrawlerManager manager, Site site)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (site == null)
                throw new ArgumentNullException(nameof(site));
            dbContext = context;
            this.site = dbContext.Sites.Attach(site);
            if (manager == null)
                throw new AgetNotInitializedException();
            this.manager = manager;
            init = true;
            log.Debug("CrawlerAgent initialized");
        }


        private CrawlDecision ShouldCrawlPage(PageToCrawl page, CrawlContext context)
        {
            if (site.Pages.FirstOrDefault(item => item.Address == page.Uri.ToString()) != null)
                return new CrawlDecision { Allow = false, Reason = "Already Crawled" };
            return new CrawlDecision { Allow = true };
        }
        private void Agent_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            log.Warn($"Page {e.CrawledPage.Uri} is Disallowed cause {e.DisallowedReason}");
            var page = site.Pages.FirstOrDefault(item => item.Address == e.CrawledPage.Uri.ToString());
            var sendNotification = false;
            if (e.DisallowedReason == "Already Crawled")
                sendNotification = true;
            if (page == null)
            {
                log.Debug("Page not found in DB. Creating new page");
                site.Pages.Add(new Page { Address = e.CrawledPage.Uri.ToString(), IsSuccess = false, SeeTime = DateTime.Now, Text = "" });
                dbContext.SaveChanges();
            }
            else
            {
                page.IsSuccess = false;
                page.SeeTime = DateTime.Now;
                dbContext.Entry(page).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            if (sendNotification)
                CallPageCrawledEvent(site, page);
        }
        private void Agent_PageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            log.Info($"Page {e.CrawledPage.Uri} crawled.");
            var crawledPage = e.CrawledPage;
            var page = site.Pages.FirstOrDefault(item => item.Address == e.CrawledPage.Uri.ToString());
            var find = false;
            if (page == null)
            {
                log.Info("Page not found in DB. Creating new page");
                page = new Page
                {
                    Address = e.CrawledPage.Uri.ToString(),
                    IsSuccess = false,
                    Text = ""
                };
            }
            else
            {
                log.Info($"Page found in Db with id {page.Id}");
                find = true;
            }
            page.SeeTime = DateTime.Now;
            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                page.IsSuccess = false;
                log.Warn($"Crawl of page failed {crawledPage.Uri}: " + crawledPage.HttpWebResponse.StatusCode, crawledPage.WebException);
            }
            else
            {
                log.Info($"Crawl of page succeeded {crawledPage.Uri}");
                page.IsSuccess = true;
                if (string.IsNullOrEmpty(crawledPage.Content.Text))
                    log.Warn($"Page had no content {crawledPage.Uri.AbsoluteUri}");
                else
                    page.Text = crawledPage.Content.Text;
            }
            if (find)
                dbContext.Entry(page).State = EntityState.Modified;
            else
                site.Pages.Add(page);
            dbContext.SaveChanges();
            CallPageCrawledEvent(site, page);
        }
        private void CallPageCrawledEvent(Site site,Page page)
        {
            if (!string.IsNullOrEmpty(page.Text))
            {
                log.Debug("Invoking PageCrawled event");
                (new Thread(() => { PageCrawled?.Invoke(site, page, page.Text); })).Start();
            }
        }
        
    }
}
