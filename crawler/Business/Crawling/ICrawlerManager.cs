using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Business.Crawling
{
    public interface ICrawlerManager
    {
        void Done(ICrawlerAgent agent);
    }
}
