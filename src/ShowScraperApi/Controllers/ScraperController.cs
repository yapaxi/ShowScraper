using Microsoft.AspNetCore.Mvc;
using ShowScraper.Api.Models;
using ShowScraper.BusinessLogic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScraper.Api.Controllers
{
    public class ScraperController : Controller
    {
        private readonly IScraperService _scraperService;

        public ScraperController(IScraperService scraperService)
        {
            _scraperService = scraperService;
        }

        [HttpPost]
        [Route("scraper/jobs")]
        public async Task<IActionResult> CreateJob([FromBody] StartScrapingRequest request)
        {
            var result = await _scraperService.CreateJob(new ScraperJobParameters(
                maxScrapers: request?.ConcurrentScrapers,
                maxShowsPerTask: request?.MaxShowsPerScraper
            ));

            return HandleResult(result, content => CreatedAtAction(nameof(Job), new { id = content.Id }, content.Id));
        }

        [HttpPost]
        [Route("scraper/jobs/{id}/executions")]
        public async Task<IActionResult> ExecuteJob(string id)
        {
            var result = await _scraperService.ExecuteJob(id);

            return HandleResult(result, content => Ok(content.ExecutionId));
        }

        [HttpGet]
        [Route("scraper/jobs/{id}")]
        public async Task<IActionResult> Job(string id)
        {
            var result = await _scraperService.GetJob(id);

            return HandleResult(result, content => Ok(content));
        }

        private IActionResult HandleResult<T>(Option<T> option, Func<T, IActionResult> onSuccess)
        {
            switch (option)
            {
                case Option<T>.Ok c:
                    return onSuccess(c.Content);
                case Option<T>.Conflict c:
                    return StatusCode(409);
                case Option<T>.NotFound c:
                    return StatusCode(404);
                case Option<T>.PreconditionViolation c:
                    return BadRequest(c.FriendlyMessage);
                default:
                    throw new InvalidOperationException($"Unexpected option type: {option?.GetType().Name ?? "<NULL>"}");
            }
        }
    }
}
