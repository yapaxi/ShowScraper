using System;

namespace ShowScraper.BusinessLogic.Contracts
{
    public class ScraperJob
    {
        public ScraperJob(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; }
    }
}
