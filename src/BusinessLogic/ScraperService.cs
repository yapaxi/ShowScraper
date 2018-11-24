using ShowScraper.BusinessLogic.Bus;
using ShowScraper.BusinessLogic.Contracts;
using ShowScraper.BusinessLogic.DataAccess;
using ShowScraper.BusinessLogic.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic
{
    public class ScraperService : IScraperService
    {
        private readonly IStorageProvider _storageProvider;
        private readonly IBus _bus;
        private readonly int _maxScrapers;

        public ScraperService(IStorageProvider storageProvider, IBus bus, int maxScrapers)
        {
            _storageProvider = storageProvider;
            _bus = bus;
            _maxScrapers = maxScrapers;
        }

        public async Task<Option<ScraperJob>> CreateJob(ScraperJobParameters scraperJob)
        {
            var maxShowsPerTask = scraperJob?.MaxShowsPerTask ?? 20;
            var startPage = scraperJob?.StartPage ?? 0;
            var endPage = scraperJob?.EndPage ?? int.MaxValue;

            if (maxShowsPerTask <= 0 || maxShowsPerTask > 250)
            {
                return new Option<ScraperJob>.PreconditionViolation($"Invalid maxium shows per task count: {maxShowsPerTask}");
            }

            if (startPage < 0)
            {
                return new Option<ScraperJob>.PreconditionViolation($"Invalid starting page: {startPage}");
            }
            
            if (startPage > endPage)
            {
                return new Option<ScraperJob>.PreconditionViolation($"Start page is greater than end page: {startPage} > {endPage}");
            }

            var job = new Job()
            {
                Id = Guid.NewGuid().ToString("N"),
                MaxShowsPerTask = maxShowsPerTask,
                StartPage = startPage,
                EndPage = endPage,
                CreatedAtUtc = DateTime.UtcNow
            };
            
            await _storageProvider.SaveJob(job);

            return new Option<ScraperJob>.Ok(Convert(job));
        }

        public async Task<Option<ScraperJobExecution>> ExecuteJob(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new Option<ScraperJobExecution>.PreconditionViolation("JobId cannot be null or empty");
            }

            var job = await _storageProvider.GetJob(id);

            if (job != null)
            {
                var executionId = await _storageProvider.TrySetExecution(job.Id);

                if (executionId == null)
                {
                    return new Option<ScraperJobExecution>.Conflict();
                }

                await _bus.SendScrapPageCommand(job.Id, job.StartPage, -1);

                return new Option<ScraperJobExecution>.Ok(new ScraperJobExecution(executionId, id));
            }
            else
            {
                return new Option<ScraperJobExecution>.NotFound();
            }
        }
        
        public async Task<Option<ScraperJob>> GetJob(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new Option<ScraperJob>.PreconditionViolation("JobId cannot be null or empty");
            }

            var job = await _storageProvider.GetJob(id);
        
            if (job != null)
            {
                return new Option<ScraperJob>.Ok(Convert(job));
            }
            else
            {
                return new Option<ScraperJob>.NotFound();
            }
        }

        private static ScraperJob Convert(Job job)
        {
            return new ScraperJob(
                id: job.Id,
                maxShowsPerTask: job.MaxShowsPerTask,
                startPage: job.StartPage,
                createdAtUtc: job.CreatedAtUtc,
                endPage: job.EndPage
            );
        }
    }
}
