using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.Contracts
{
    public interface IScraperService
    {
        Task<Option<ScraperJob>> StartJob(ScraperJobParameters scraperJob);
        Task<Option<ScraperJob>> GetJob(string id);
    }
}
