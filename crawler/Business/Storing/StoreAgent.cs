using System;
using System.Collections.Generic;
using Crawler.Model;
using System.IO;
using System.Text;
using System.Linq;

namespace Crawler.Business.Storing
{
    public class StoreAgent : IStoreAgent
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(StoreAgent));
        string basePath;
        List<string> headers = new List<string>();
        private StreamWriter writer;
        bool init;

        public void Init(long categoryId, string basePath)
        {
            using (var cnx = new ApplicationDbContext())
            {
                var category = cnx.Categories.FirstOrDefault(m => m.Id == categoryId);
                if (category == null)
                    throw new ArgumentNullException(nameof(category));
                if (string.IsNullOrEmpty(basePath))
                    throw new ArgumentNullException(nameof(basePath));
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                this.basePath = basePath;
                if (category.Filters == null || category.Filters.Count == 0)
                    throw new StoreNotInitializedException("Category has no filters");
                foreach (var item in category.Filters)
                    headers.Add(item.OutName);
                if (!(basePath.EndsWith("/") || basePath.EndsWith("\\")))
                    basePath += "/";
                var path = basePath + category.Name + ".csv";
                var hasHeader = false;
                if (File.Exists(path))
                {
                    using (var streamReader = new StreamReader(path))
                    {
                        if (!string.IsNullOrEmpty(streamReader.ReadLine()))
                            hasHeader = true;
                    }
                }
                writer = new StreamWriter(File.OpenWrite(path))
                {
                    AutoFlush = true

                };
                writer.BaseStream.Seek(0, SeekOrigin.End);
                if (!hasHeader)
                    WriteRow(headers);
                init = true;
            }
        }

        public void Store(Dictionary<string, string> record)
        {
            if (!init)
                throw new StoreNotInitializedException();
            var row = new List<string>();
            foreach (var field in headers)
            {
                var value = "";
                if (record.TryGetValue(field, out value))
                {
                    value = value.Replace('\r', ' ').Replace('\n',' ');
                    row.Add(value);
                }
                else
                    row.Add(" ");
            }
            WriteRow(row);
        }
        private void WriteRow(List<string> row)
        {
            var builder = new StringBuilder();
            var firstColumn = true;
            foreach (string value in row)
            {
                if (!firstColumn)
                    builder.Append(',');
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            writer.WriteLine(builder.ToString());
        }

        public void Close()
        {
            writer.Flush();
            writer.Close();
            writer.Dispose();
        }
    }
}
