using ShowScraper.BusinessLogic.DataAccess.Model;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.DataAccess
{
    public interface IStorageProvider
    {
        Task<Job> GetJob(string id);
        Task<JobTask> GetTask(string id);
        Task SaveJob(Job job);
        Task SaveTask(JobTask task);
    }
}
