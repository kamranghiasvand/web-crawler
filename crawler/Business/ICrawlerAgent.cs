using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.Business
{
    public interface ICrawlerAgent
    {
        void Start();
        void Stop();
        string GetId();
    }
}
