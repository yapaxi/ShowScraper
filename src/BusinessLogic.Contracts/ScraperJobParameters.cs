namespace ShowScraper.BusinessLogic.Contracts
{
    public class ScraperJobParameters
    {
        public ScraperJobParameters(int? maxShowsPerTask = null, int? maxScrapers = null)
        {
            MaxShowsPerTask = maxShowsPerTask;
            MaxScrapers = maxScrapers;
        }

        public int? MaxShowsPerTask { get; }
        public int? MaxScrapers { get; }
    }
}
