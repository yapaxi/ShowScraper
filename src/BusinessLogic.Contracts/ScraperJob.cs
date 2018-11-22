using System;
using System.Collections.Generic;

namespace ShowScraper.BusinessLogic.Contracts
{
    public class ScraperJob
    {
        public ScraperJob(string id, int maxShowsPerTask, int startingPage, DateTime createdAtUtc)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            MaxShowsPerTask = maxShowsPerTask;
            StartingPage = startingPage;
            CreatedAtUtc = createdAtUtc;
        }

        public string Id { get; }
        public int MaxShowsPerTask { get; }
        public int StartingPage { get; }
        public DateTime CreatedAtUtc { get; }
    }
}
