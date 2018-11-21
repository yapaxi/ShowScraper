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
    }
}
