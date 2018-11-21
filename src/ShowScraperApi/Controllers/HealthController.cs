using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScraper.Api.Controllers
{
    public class HealthController : Controller
    {
        [HttpGet]
        [Route("health")]
        public IActionResult Health() => Ok();
    }
}
