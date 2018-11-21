using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowScraper.BusinessLogic.DataAccess.Model
{
    [DynamoDBTable("Scraper.Tasks")]
    public class JobTask
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string JobId { get; set; }

        [DynamoDBProperty]
        public string ScraperId { get; set; }

        [DynamoDBProperty]
        public List<string> Shows { get; set; }

        [DynamoDBProperty]
        public bool IsComplete { get; set; }
    }
}
