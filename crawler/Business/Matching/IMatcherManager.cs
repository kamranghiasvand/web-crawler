﻿using crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.Business.Matching
{
    public interface IMatcherManager
    {
        void MatchFound(IMatcher matcher, Category category, Dictionary<string, string> record);
        void Init(ApplicationDbContext context);
    }
}