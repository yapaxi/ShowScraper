using System;
using System.Collections.Generic;

namespace ShowScraper.BusinessLogic.Contracts
{
    public class ScraperJob
    {
        public ScraperJob(string id, int maxShowsPerTask, int startPage, int endPage, DateTime createdAtUtc)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            MaxShowsPerTask = maxShowsPerTask;
            StartPage = startPage;
            EndPage = endPage;
            CreatedAtUtc = createdAtUtc;
        }

        public string Id { get; }
        public int MaxShowsPerTask { get; }
        public int StartPage { get; }
        public int EndPage { get; }
        public DateTime CreatedAtUtc { get; }
    }
}
