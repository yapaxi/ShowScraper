using ShowScraper.BusinessLogic.Contracts;
using ShowScraper.BusinessLogic.DataAccess;
using ShowScraper.BusinessLogic.DataAccess.Model;
using ShowScraper.BusinessLogic.TVMaze;
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
        private readonly IShowDatabase _showDatabase;
        private readonly int _maxScrapers;

        public ScraperService(IStorageProvider storageProvider, IShowDatabase showDatabase, int maxScrapers)
        {
            _storageProvider = storageProvider;
            _showDatabase = showDatabase;
            _maxScrapers = maxScrapers;
        }

        public async Task<Option<ScraperJob>> CreateJob(ScraperJobParameters scraperJob)
        {
            var maxScrapers = scraperJob?.MaxScrapers ?? 2;
            var maxShowsPerTask = scraperJob?.MaxShowsPerTask ?? 10;

            if (maxScrapers <= 0)
            {
                return new Option<ScraperJob>.PreconditionViolation($"Invalid maxium scraper count: {maxScrapers}");
            }

            if (maxShowsPerTask <= 0)
            {
                return new Option<ScraperJob>.PreconditionViolation($"Invalid maxium shows per task count: {maxShowsPerTask}");
            }

            if (maxScrapers > _maxScrapers)
            {
                return new Option<ScraperJob>.PreconditionViolation($"Provided scraper count is greater that maxium: {maxScrapers} > {_maxScrapers}");
            }

            var job = new Job()
            {
                Id = Guid.NewGuid().ToString("N"),
                MaxScrapers = maxScrapers,
                MaxShowsPerTask = maxShowsPerTask,
                AssignedScrapers = new List<string>(),
                CreatedAtUtc = DateTime.UtcNow
            };

            await _storageProvider.SaveJob(job);

            var allShows = await _showDatabase.GetAllShows();
            
            var assignments = (
                 from q in allShows.Select((show, index) => (show, index))
                 group q.show by q.index % maxScrapers into scrapers
                 let scraperId = Guid.NewGuid().ToString("N")
                 let tasks = (from z in scrapers.Select((shows, index) => (shows, index))
                              group z.shows by z.index / maxShowsPerTask into grp
                              let taskId = Guid.NewGuid().ToString("N")
                              select new JobTask()
                              {
                                  Id = $"{job.Id}:{scraperId}:{grp.Key}",
                                  JobId = job.Id,
                                  ScraperId = scraperId,
                                  Shows = grp.Select(q => q.Id.ToString()).ToList()
                              }).ToArray()
                 select new
                 {
                     scraperId,
                     tasks
                 }
            ).ToArray();

            foreach (var scraper in assignments)
            {
                foreach (var task in scraper.tasks)
                {
                    await _storageProvider.SaveTask(task);
                }

                job.AssignedScrapers.Add(scraper.scraperId);
            }
            
            await _storageProvider.SaveJob(job);

            return new Option<ScraperJob>.Ok(Convert(job));
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
                maxScrapers: job.MaxScrapers,
                assignedScrapers: job.AssignedScrapers,
                createdAtUtc: job.CreatedAtUtc
            );
        }
    }
}
