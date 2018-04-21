using System;
using Crawler.Business.Crawling;
using Crawler.Model;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Abot.Poco;
using System.Linq;
using AngleSharp.Dom;
using System.Text.RegularExpressions;

namespace Crawler.Business.Matching
{
    public class Matcher : IMatcher
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Matcher));
        static readonly Regex trimmer = new Regex(@"\s\s+");
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

        public void Init(ICrawlerAgent agent, IMatcherManager manager)
        {
            log.Debug("Initializing Matcher");
            if (agent == null)
                throw new ArgumentNullException(nameof(agent));
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            this.agent = agent;
            this.manager = manager;
            this.agent.PageCrawled += Agent_PageCrawledAsync;
            log.Debug($"Attached event listener to PageCrawled of agent with id {this.agent.GetId()}");
            init = true;
        }

        private Task Agent_PageCrawledAsync(long siteId, long pageId, string text, CrawledPage crawledPage)
        {
            if (!init)
                throw new MatcherNotInitializedException();
            return Task.Run(() =>
            {
                using (var context = new ApplicationDbContext())
                {
                    var site = context.Sites.FirstOrDefault(m => m.Id == siteId);
                    if (site == null)
                        throw new ArgumentNullException(nameof(site));
                    var page = site.Pages.FirstOrDefault(m => m.Id == pageId);
                    if (page == null)
                        throw new ArgumentNullException(nameof(page));
                    if (crawledPage == null)
                        throw new ArgumentNullException(nameof(crawledPage));

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
                            context.Entry(page).State = System.Data.Entity.EntityState.Modified;
                            log.Warn($"Category with id {cat.Id} definition has no filter");
                            continue;
                        }
                        var record = new Dictionary<string, string>();

                        #region Check Filter and Criteria

                        if (CheckCriteria(crawledPage, cat.Id))
                            record = CheckFilters(crawledPage, cat.Id);
                        #endregion

                        AddIdToCategoriesList(page, cat.Id);
                        context.Entry(page).State = System.Data.Entity.EntityState.Modified;
                        LogRecord(record);
                        CallRecordFound(cat, record);
                        log.Debug($"Done check category with id {cat.Id}");
                    }
                }
            });

        }
        private static bool CheckCriteria(CrawledPage page, long catId)
        {
            using (var context = new ApplicationDbContext())
            {
                var cat = context.Categories.FirstOrDefault(m => m.Id == catId);
                if (cat.Criteria.Count <= 0)
                {
                    log.Warn($"category with id {cat.Id} has no criteria.");
                    return false;
                }
                foreach (var cri in cat.Criteria)
                {
                    try
                    {
                        log.Debug($"Trying criteria with id {cri.Id}");
                        if (string.IsNullOrEmpty(cri.Selector))
                        {
                            log.Warn($"Filter with id {cri.Id} has no selector.");
                            return false;
                        }
                        var node = page.AngleSharpHtmlDocument.QuerySelector(cri.Selector);
                        if (node != null)
                        {
                            log.Debug("node found");
                            if (cri.Location == Location.Attribute)
                            {
                                var attr = node.Attributes[cri.Name];
                                log.Debug($"Found value  [{attr.Value}] for criteria with id {cri.Id}");
                                if (attr.Value.Trim() != cri.ExpectedValue)
                                {
                                    log.Debug($"attribute with value [{attr.Value}] not match with criteria id {cri.Id}");
                                    return false;
                                }
                            }
                            else if (cri.Location == Location.InnerText)
                            {
                                log.Debug($"Found value  [{node.InnerHtml}]  for criteria with id {cri.Id}");
                                if (node.InnerHtml.Trim() != cri.ExpectedValue)
                                {
                                    log.Debug($"node with innerHtml [{node.InnerHtml}] not match with criteria id {cri.Id}");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            log.Debug($"node not found");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message, ex);
                        return false;
                    }

                }
            }
            return true;
        }
        private static Dictionary<string, string> CheckFilters(CrawledPage page, long catId)
        {
            var record = new Dictionary<string, string>();
            using (var context = new ApplicationDbContext())
            {
                var cat = context.Categories.FirstOrDefault(m => m.Id == catId);
                foreach (var filter in cat.Filters)
                {
                    try
                    {
                        log.Debug($"Trying filter with id {filter.Id}");
                        if (string.IsNullOrEmpty(filter.Selector))
                        {
                            log.Warn($"Filter with id {filter.Id} has no selector.");
                            continue;
                        }
                        var node = page.AngleSharpHtmlDocument.QuerySelector(filter.Selector);
                        if (GetValue(node, filter) != null)
                            record.Add(filter.OutName, node.InnerHtml);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message, ex);
                    }
                }
            }
            return record;
        }
        private static string GetValue(IElement node, Filter filter)
        {
            string result = null;
            if (node != null)
            {
                log.Debug("node found");
                if (filter.Location == Location.Attribute)
                {
                    var attr = node.Attributes[filter.Name];
                    result = attr.Value;
                    result = Trim(result);
                    if (filter.ValueRegex != null)
                    {
                        var res = Regex.Match(result, filter.ValueRegex);
                        result = res.Value;
                    }
                    else
                        result = attr.Value;

                }
                else if (filter.Location == Location.InnerText)
                {
                    result = node.InnerHtml;
                    result = Trim(result);
                    if (filter.ValueRegex != null)
                    {
                        var res = Regex.Match(result, filter.ValueRegex);
                        result = res.Value;
                    }
                    else
                        result = node.InnerHtml;
                }
                log.Debug($"Found value  [{result}] for filter with id {filter.Id}");
                return result;
            }
            else
            {
                log.Debug($"node not found");
                return null;
            }
        }
        private static string Trim(string value)
        {
            if (value == null)
                return null;
            value = value.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').Trim();
            value = trimmer.Replace(value, " ");
            return value;

        }
        private static void AddIdToCategoriesList(Page page, long catId)
        {
            if (page == null)
                return;
            if (page.CategoriesId == null)
                page.CategoriesId = new List<long>();
            page.CategoriesId.Add(catId);

        }
        private static void LogRecord(Dictionary<string, string> record)
        {
            if (record == null)
            {
                log.Warn("Record is null");
                return;
            }
            if (record.Count == 0)
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
