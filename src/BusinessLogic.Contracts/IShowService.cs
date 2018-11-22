using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.Contracts
{
    public interface IShowService
    {
        Task<IReadOnlyList<string>> GetShows(int page);
    }
}
