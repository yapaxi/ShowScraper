using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScraper.Api.Models
{
    public class StartScrapingRequest
    {
        public int? StartPage { get; set; }
        public int? MaxShowsPerScraper { get; set; }
        public int? EndPage { get; set; }
    }
}
