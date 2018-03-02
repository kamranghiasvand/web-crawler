using Crawler.Business.Crawling;
using Crawler.Business.Matching;
using Crawler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            if (isRunning)
            {
                log.Warn("Engine is already started");
                return;
            }
            isRunning = true;
            CheckJsonFile();
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
            catch (Exception ex)
            {
                log.Fatal(ex.Message, ex);
            }
        }
        public void Stop()
        {
            if (!isRunning)
            {
                log.Warn("Engine is already stopped");
                return;
            }
            isRunning = false;
            foreach (var proc in processors)
                proc.Stop();
        }

        private  void CheckJsonFile()
        {
            if (!bool.Parse(ConfigurationManager.AppSettings["checkInputJson"]))
                return;
            var path = ConfigurationManager.AppSettings["jsonFile"];
            if (string.IsNullOrEmpty(path))
                return;
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
                return;
            try
            {
                var sites = JsonConvert.DeserializeObject<List<Site>>(File.ReadAllText(path));
                foreach (var site in sites)
                    AddNewSite(site);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }


        }
        public void AddNewSite(Site item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            log.Debug("Adding New site");
            var site = context.Sites.FirstOrDefault(m =>m.Equals(item));
            SiteProcessor proc = null;
            if (site == null)
            {
                log.Debug("Site not found. Adding Site to database");
                context.Sites.Add(item);
                context.SaveChanges();
                proc = new SiteProcessor(context, site);
            }
            else
            {
                log.Debug("Find site in database. checking for changes");
                proc = processors.FirstOrDefault(m => m.GetBaseUrl == site.BaseUrl);
                if(proc==null)
                {
                    log.Warn($"processor not found for site with id {site.Id}");
                    log.Debug($"creating processor for site with id {site.Id}");
                    proc = new SiteProcessor(context, site);
                }
                proc.Stop();
                site.Categories.Clear();
                foreach (var cat in item.Categories)
                    site.Categories.Add(cat);
                context.Entry(site).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
               

            }
            proc.Start();
           
        }

    }
}
