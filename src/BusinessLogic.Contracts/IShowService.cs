using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.Contracts
{
    public interface IShowService
    {
        Task<JArray> GetShows(int page);
    }
}
