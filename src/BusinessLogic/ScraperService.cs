using ShowScraper.BusinessLogic.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic
{
    public class ScraperService : IScraperService
    {
        public async Task<Option<ScraperJob>> StartJob(ScraperJobParameters scraperJob)
        {
            return new Option<ScraperJob>.Ok(new ScraperJob(Guid.NewGuid().ToString("N")));
        }

        public async Task<Option<ScraperJob>> GetJob(string id)
        {
            return new Option<ScraperJob>.Ok(new ScraperJob(id));
        }
    }
}
