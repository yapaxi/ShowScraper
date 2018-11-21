using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScraper.Api.Models
{
    public class StartScrapingRequest
    {
        public int? ConcurrentScrapers { get; set; }
        public int? MaxShowsPerScraper { get; set; }
    }
}
