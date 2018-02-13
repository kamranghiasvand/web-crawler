﻿using Abot.Crawler;
using crawler.Business;
using crawler.Model;
using System.Collections.Generic;

namespace crawler.business
{
    public class CrawlerManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CrawlerManager));
        private List<ICrawlerAgent> agents;
        private ApplicationDbContext context;

        public void Start()
        {
            log.Debug("starting CrawlerManager");


        }
        public void Stop()
        {

        }
    }
}