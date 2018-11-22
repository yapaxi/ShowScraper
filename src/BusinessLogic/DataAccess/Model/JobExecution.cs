using Amazon.DynamoDBv2.DataModel;

namespace ShowScraper.BusinessLogic.DataAccess.Model
{
    [DynamoDBTable("Scraper.Executions")]
    public class JobExecution
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string JobId { get; set; }

        [DynamoDBProperty]
        public string ExecutionId { get; set; }

        [DynamoDBVersion]
        public int? Version { get; set; }
    }
}
