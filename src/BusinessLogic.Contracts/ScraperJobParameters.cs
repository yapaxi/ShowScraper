namespace ShowScraper.BusinessLogic.Contracts
{
    public class ScraperJobParameters
    {
        public ScraperJobParameters(int? maxShowsPerTask = null, int? startingPage = null)
        {
            MaxShowsPerTask = maxShowsPerTask;
            StartingPage = startingPage;
        }

        public int? MaxShowsPerTask { get; }
        public int? StartingPage { get; }
    }
}
