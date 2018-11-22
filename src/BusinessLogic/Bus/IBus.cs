using ShowScraper.BusinessLogic.DataAccess.Model;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.Bus
{
    public interface IBus
    {
        Task SendScrapPageCommand(string jobId, int pageId, int lastId);
    }
}
