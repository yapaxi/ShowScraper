using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScraper.Api.Controllers
{
    public class ShowsController : Controller
    {
        [HttpGet]
        [Route("shows")]
        public async Task<IActionResult> Get([FromQuery] int page = 0)
        {
            return Ok(page);
        }
    }
}
