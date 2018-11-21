using Microsoft.AspNetCore.Mvc;
using ShowScraper.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScraper.Api.Controllers
{
    public class ScraperController : Controller
    {
        [HttpPost]
        [Route("scraper/jobs")]
        public async Task<IActionResult> StartScraping([FromBody] StartScrapingRequest request)
        {
            var id = Guid.NewGuid().ToString("N");

            return CreatedAtAction(nameof(Job), new { id = id }, request);
        }

        [HttpGet]
        [Route("scraper/jobs/{id}")]
        public async Task<IActionResult> Job(string id)
        {
            return Ok(new { xxx = "aaa" });
        }
    }
}
