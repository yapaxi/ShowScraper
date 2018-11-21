using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowScraper.BusinessLogic.DataAccess.Model
{
    [DynamoDBTable("Scraper.Jobs")]
    public class Job
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public int MaxShowsPerTask { get; set; }

        [DynamoDBProperty]
        public int MaxScrapers { get; set; }

        [DynamoDBProperty]
        public List<string> AssignedScrapers { get; set; }

        [DynamoDBProperty]
        public DateTime CreatedAtUtc { get; set; }
    }
}
