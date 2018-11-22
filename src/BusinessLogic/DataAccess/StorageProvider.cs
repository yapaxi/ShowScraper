using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json.Linq;
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
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;

        public StorageProvider(IDynamoDBContext context, AmazonDynamoDBClient amazonDynamoDBClient)
        {
            _context = context;
            _amazonDynamoDBClient = amazonDynamoDBClient;
        }

        public Task<Job> GetJob(string id)
        {
            return _context.LoadAsync<Job>(id, new DynamoDBOperationConfig()
            {
                ConsistentRead = true
            });
        }

        public Task SaveJob(Job job)
        {
           return _context.SaveAsync(job);
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

        public async Task<JArray> GetShows(int page)
        {
            Dictionary<string, AttributeValue> lastKey = null;

            var array = new JArray();

            do
            {
                var result = await _amazonDynamoDBClient.QueryAsync(new QueryRequest("Scraper.Shows")
                {
                    KeyConditionExpression = "pageId = :p1",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        {":p1", new AttributeValue() { N = page.ToString() } }
                    },
                    ExclusiveStartKey = lastKey
                });
                
                foreach (var r in result.Items)
                {
                    var doc = Document.FromAttributeMap(r);
                    var obj = JToken.Parse(doc.ToJson());
                    array.Add(obj);
                }

                lastKey = result.LastEvaluatedKey;
            }
            while (lastKey != null && lastKey.Count != 0);

            return array;
        }

        public Task ResetExecution()
        {
            return _context.DeleteAsync("1");
        }
    }
}
