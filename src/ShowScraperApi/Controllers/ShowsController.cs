using Microsoft.AspNetCore.Mvc;
using ShowScraper.BusinessLogic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.Api.Controllers
{
    public class ShowsController : Controller
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpGet]
        [Route("shows")]
        public async Task<IActionResult> Get([FromQuery] int page = 0)
        {
            var shows = await _showService.GetShows(page);

            return Content(shows.ToString(Newtonsoft.Json.Formatting.Indented), "application/json", Encoding.UTF8);
        }
    }
}
