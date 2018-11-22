using System;

namespace ShowScraper.BusinessLogic.Contracts
{
    public class ScraperJobExecution
    {
        public ScraperJobExecution(string executionId, string jobId)
        {
            ExecutionId = executionId ?? throw new ArgumentNullException(nameof(executionId));
            JobId = jobId ?? throw new ArgumentNullException(nameof(jobId));
        }

        public string ExecutionId { get; }
        public string JobId { get; }
    }
}
