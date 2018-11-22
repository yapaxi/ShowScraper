using Amazon.DynamoDBv2.DataModel;
using ShowScraper.BusinessLogic.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.DataAccess
{
    public class StorageProvider: IStorageProvider
    {
        private readonly IDynamoDBContext _context;

        public StorageProvider(IDynamoDBContext context)
        {
            _context = context;
        }

        public Task<Job> GetJob(string id)
        {
            return _context.LoadAsync<Job>(id, new DynamoDBOperationConfig()
            {
                ConsistentRead = true
            });
        }

        public Task<JobTask> GetTask(string id)
        {
            return _context.LoadAsync<JobTask>(id, new DynamoDBOperationConfig()
            {
                ConsistentRead = true
            });
        }

        public Task SaveJob(Job job)
        {
           return _context.SaveAsync(job);
        }

        public Task SaveTask(JobTask task)
        {
            return _context.SaveAsync(task);
        }

        public async Task<string> TrySetExecution(string jobId)
        {
            var currentExecution = await _context.LoadAsync<JobExecution>("1");

            if (currentExecution != null)
            {
                return null;
            }

            var executionId = Guid.NewGuid().ToString("N");

            var execution = new JobExecution()
            {
                Id = "1",
                JobId = jobId,
                ExecutionId = executionId
            };

            await _context.SaveAsync(execution);

            return executionId;
        }

        public Task ResetExecution()
        {
            return _context.DeleteAsync("1");
        }
    }
}
