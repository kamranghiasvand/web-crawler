using crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.Business.Crawling
{
    public interface ICrawlerManager
    {
        void Init(ApplicationDbContext context);
        void Done(ICrawlerAgent agent);
        void Start();
        void Stop();
        void AddNewSite(Site site);
    }
}
