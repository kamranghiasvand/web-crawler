using Abot.Crawler;
using Abot.Poco;
using AbotX.Crawler;
using crawler.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace crawler.Business
{
    public class CrawlerAgent : ICrawlerAgent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CrawlerAgent));
        private readonly ApplicationDbContext dbContext;
        private readonly Site site;
        private readonly string guid;
        private CrawlerX agent;
        private bool isRunning;

        public event Func<string, string, Task> PageCrawled;

        public CrawlerAgent(ApplicationDbContext context, Site site)
        {
            dbContext = context;
            this.site = dbContext.Sites.Attach(site);
            guid = Guid.NewGuid().ToString();
            log.Debug($"Crawler agent with id {guid} was created");

        }

        public string GetId()
        {
            return guid;
        }
        public void Start()
        {
            lock (this)
            {
                log.Debug($"Starting crawler with id {guid}");
                if (isRunning)
                {
                    log.Info($"Crawler with id {guid} is already started");
                    return;
                }
                isRunning = true;
                agent = new CrawlerX();
                agent.PageCrawlCompletedAsync += Agent_PageCrawlCompleted;
                agent.PageLinksCrawlDisallowedAsync += Agent_PageLinksCrawlDisallowed;
                agent.ShouldCrawlPage(ShouldCrawlPage);
                (new Thread(() =>
                {
                    agent.Crawl(new Uri(site.BaseUrl));
                    lock(this)  isRunning = false;

                })).Start();
            }

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
            if (page == null)
            {
                site.Pages.Add(new Page { Address = e.CrawledPage.Uri.ToString(), IsSuccess = false, SeeTime = DateTime.Now, Text = "" });
            }
            else
            {
                page.IsSuccess = false;
                page.SeeTime = DateTime.Now;
                dbContext.Entry(page).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        private void Agent_PageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            var crawledPage = e.CrawledPage;
            var page = site.Pages.FirstOrDefault(item => item.Address == e.CrawledPage.Uri.ToString());
            var find = false;
            if (page == null)
            {
                page = new Page
                {
                    Address = e.CrawledPage.Uri.ToString(),
                    IsSuccess = false,
                    Text = ""
                };
            }
            else
            {
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
            if (!string.IsNullOrEmpty(page.Text))
            {
                (new Thread(() => { PageCrawled?.Invoke(page.Address, page.Text); })).Start();
            }
        }
        public void Stop()
        {
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
            return isRunning;
        }
    }
}
