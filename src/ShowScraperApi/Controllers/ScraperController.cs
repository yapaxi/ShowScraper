using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScraper.Api.Controllers
{
    public class ScraperController : Controller
    {
        [HttpGet]
        [Route("scraper")]
        public async Task<IActionResult> GetStuff()
        {
            return Ok("success");
        }
    }
}
