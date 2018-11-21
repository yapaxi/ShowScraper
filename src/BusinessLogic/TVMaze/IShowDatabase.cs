using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.TVMaze
{
    public interface IShowDatabase
    {
        Task<IReadOnlyList<Show>> GetAllShows();
    }
}
