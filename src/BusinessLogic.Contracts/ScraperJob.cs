using System;
using System.Collections.Generic;

namespace ShowScraper.BusinessLogic.Contracts
{
    public class ScraperJob
    {
        public ScraperJob(string id, int maxShowsPerTask, int maxScrapers, IReadOnlyList<string> assignedScrapers, DateTime createdAtUtc)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            MaxShowsPerTask = maxShowsPerTask;
            MaxScrapers = maxScrapers;
            AssignedScrapers = assignedScrapers ?? throw new ArgumentNullException(nameof(assignedScrapers));
            CreatedAtUtc = createdAtUtc;
        }

        public string Id { get; }
        public int MaxShowsPerTask { get; }
        public int MaxScrapers { get; }
        public IReadOnlyList<string> AssignedScrapers { get; }
        public DateTime CreatedAtUtc { get; }
    }
}
