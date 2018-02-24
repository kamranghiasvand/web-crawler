using Abot.Poco;
using AbotX.Crawler;
using Crawler.Business;
using Crawler.Business.Storing;
using Crawler.Model;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Crawler
{

    class Program
    {
        static ApplicationDbContext dbContext = new ApplicationDbContext();

        static void Main(string[] args)
        {
            var log = log4net.LogManager.GetLogger(typeof(Program));

            log.Debug("Starting Application");
            Category cat = null;
            Site site = null;
            var context = new ApplicationDbContext();
            site = context.Sites.FirstOrDefault(m => m.Name == "Rokoland");
            if (site == null)
            {
                cat = new Category
                {
                    Name = "Rokoland_IceCream"

                };
                var filter = new Filter
                {
                    Location = Location.InnerText,
                    Name = "h1",
                    XPath = "//*[@id=\"product-details-form\"]/div/div[1]/div[2]/div[1]/h1",                   
                    OutName = "Detail",
                    Type = Model.ValueType.txt
                };
                cat.Filters.Add(filter);

                filter = new Filter
                {
                    Location = Location.Attribute,
                    Name = "content",
                    OutName = "Price",
                    Type = Model.ValueType.txt,
                    XPath = "//*[@id=\"product-details-form\"]/div/div[1]/div[2]/div[5]/div/span"
                };
             
                cat.Filters.Add(filter);

                filter = new Filter
                {
                    Location = Location.Attribute,
                    Name = "src",
                    OutName = "Image",
                    Type = Model.ValueType.txt,
                    XPath = "//*[@id=\"product-details-form\"]/div/div[1]/div[1]/div/img"
                };
                cat.Filters.Add(filter);

                site = new Site { BaseUrl = "http://www.rocoland.com/", OutputFolder = Directory.GetCurrentDirectory() + "/rokoland", Name = "Rokoland" };
                site.Categories.Add(cat);
                context.Sites.Add(site);
                context.SaveChanges();
            }
            var engine = new Engine(context);
            engine.Start();
            //Thread.Sleep(1 * 60 * 1000);
            //engine.Stop();
            Console.ReadKey();
        }
    }


    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }



    /// <summary>
    /// Class to read data from a CSV file
    /// </summary>
    public class CsvFileReader : StreamReader
    {
        public CsvFileReader(Stream stream)
            : base(stream)
        {
        }

        public CsvFileReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;

            var pos = 0;
            var rows = 0;

            while (pos < row.LineText.Length)
            {
                string value;

                // Special handling for quoted field
                if (row.LineText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    var start = pos;
                    while (pos < row.LineText.Length)
                    {
                        // Test for quote character
                        if (row.LineText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= row.LineText.Length || row.LineText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    var start = pos;
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    value = row.LineText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }
            // Delete any unused items
            while (row.Count > rows)
                row.RemoveAt(rows);

            // Return true if any columns read
            return (row.Count > 0);
        }
    }
}
