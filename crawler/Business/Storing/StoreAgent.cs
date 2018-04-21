using System;
using System.Collections.Generic;
using Crawler.Model;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Crawler.Business.Storing
{
    public class StoreAgent : IStoreAgent
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(StoreAgent));
        string basePath;
        List<HeaderType> headers = new List<HeaderType>();
        static readonly Regex trimmer = new Regex(@"\s\s+");
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
                if (category.Filters == null || category.Filters.Count == 0)
                    throw new StoreNotInitializedException("Category has no filters");
                foreach (var item in category.Filters)
                    headers.Add(new HeaderType { headerName = item.OutName, Type = item.Type });
                if (!(basePath.EndsWith("/") || basePath.EndsWith("\\")))
                    basePath += "/";
                this.basePath = basePath;
                if (!Directory.Exists(basePath + "Images/"))
                    Directory.CreateDirectory(basePath + "Images/");
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
                writer = new StreamWriter(File.OpenWrite(path), Encoding.UTF8)
                {
                    AutoFlush = true

                };
                writer.BaseStream.Seek(0, SeekOrigin.End);
                if (!hasHeader)
                    WriteRow(headers.Select(m => m.headerName).ToList());
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
                if (record.TryGetValue(field.headerName, out value))
                {
                    if (field.Type == Model.ValueType.txt)
                    {
                        value = value.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').Trim();
                        value = trimmer.Replace(value, " ");
                        row.Add(value);
                    }
                    else if (field.Type == Model.ValueType.jpeg)
                    {
                        var name = Path.GetRandomFileName() + ".jpeg";
                        SaveImage(value, basePath + "Images/", name);
                        row.Add(basePath + "Images/" + name);

                    }
                }
                else
                    row.Add(" ");
            }
            WriteRow(row);
        }

        private static void SaveImage(string url, string directory, string name)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(url), directory + name);
            }
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
            //  writer.Close();
            //  writer.Dispose();
        }
    }
    public class HeaderType
    {
        public String headerName { get; set; }
        public Model.ValueType Type { get; set; }
    }
}
