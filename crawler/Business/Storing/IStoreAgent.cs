using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Business.Storing
{
    public interface IStoreAgent
    {
        void Init(ApplicationDbContext context, Category category, string basePath);
        void Store(Dictionary<string, string> record);
        void Close();
    }
    [Serializable]
    public class StoreNotInitializedException : Exception {
        public StoreNotInitializedException() : base() { }
        public StoreNotInitializedException(string message) : base(message) { }
    }
}
