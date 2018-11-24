namespace ShowScraper.BusinessLogic.Contracts
{
    public class ScraperJobParameters
    {
        public ScraperJobParameters(int? maxShowsPerTask = null, int? startPage = null, int? endPage = null)
        {
            MaxShowsPerTask = maxShowsPerTask;
            StartPage = startPage;
            EndPage = endPage;
        }

        public int? MaxShowsPerTask { get; }
        public int? StartPage { get; }
        public int? EndPage { get; }
    }
}
