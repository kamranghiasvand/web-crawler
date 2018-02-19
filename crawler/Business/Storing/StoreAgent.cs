using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.Model;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Crawler.Business.Storing
{
    public class StoreAgent : IStoreAgent, StreamWriter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(StoreAgent));
        ApplicationDbContext context;
        Category category;
        string basePath;
        bool init;
        CsvWriter writer;
        object recordObj;
        List<string> headers = new List<string>();

        public void Init(ApplicationDbContext context, Category category, string basePath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentNullException(nameof(basePath));
            if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException();
            this.context = context;
            this.category = category;
            this.basePath = basePath;
        
            if (category.Filters == null || category.Filters.Count == 0)
                throw new StoreNotInitializedException("Category has no filters");
            foreach (var item in category.Filters)
                headers.Add(item.OutName);        
            var config = new Configuration { AllowComments = false, HasHeaderRecord = true, TrimOptions = TrimOptions.Trim };
            writer = new CsvWriter(new StreamWriter(File.OpenWrite(basePath + "//" + category.Name + ".csv")), config);
            writer.WriteHeader(recordObj.GetType());
            init = true;
        }

        public void Store(Dictionary<string, string> record)
        {
            if (!init)
                throw new StoreNotInitializedException();
            foreach (var filed in record)
                recordObj.GetType().GetProperty(filed.Key).SetValue(recordObj, filed.Value);
            writer.WriteRecord(recordObj);
            writer.Flush();

        }      
    }
}
