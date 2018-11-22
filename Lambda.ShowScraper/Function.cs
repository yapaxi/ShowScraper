using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using System.Net.Http;
using System.Net;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json.Linq;
using Amazon.SimpleNotificationService;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lambda.ShowScraper
{
    public class Function
    {
        private static readonly AmazonDynamoDBClient _client = new AmazonDynamoDBClient();
        private static readonly DynamoDBContext _context = new DynamoDBContext(_client);
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly IAmazonSimpleNotificationService _sns = new AmazonSimpleNotificationServiceClient();

        public async Task<string> Handle(SNSEvent snsEvent)
        {
            var record = snsEvent.Records.Single();

            var message = JObject.Parse(record.Sns.Message);
            var jobId = message["jobId"].Value<string>();
            var pageId = message["pageId"].Value<int>();
            var lastId = message["lastId"].Value<int>();

            var job = await _context.LoadAsync<Job>(jobId);

            if (job == null)
            {
                return "job-not-found";
            }

            var pageUrl = $"http://api.tvmaze.com/shows?page={pageId}";
            var pageResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, pageUrl));

            if (pageResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return "end";
            }

            var shows = JArray.Parse(await pageResponse.Content.ReadAsStringAsync());
            
            var counter = 0;
            int? lastShowId = null;
            var pageIncomplete = false;
            foreach (var showBrief in shows.OrderBy(e => e["id"].Value<int>()))
            {
                var showId = showBrief["id"].Value<int>();

                if (showId <= lastId)
                {
                    continue;
                }

                if (++counter >= job.MaxShowsPerTask)
                {
                    pageIncomplete = true;
                    break;
                }

                var url = $"http://api.tvmaze.com/shows/{showId}?embed=cast";
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

                response.EnsureSuccessStatusCode();

                var show = JObject.Parse(await response.Content.ReadAsStringAsync());
                var doc = new JObject();
                doc["id"] = showId;
                doc["pageId"] = pageId;
                doc["name"] = show["name"];
                doc["cast"] = show["_embedded"]["cast"];
                var table = Table.LoadTable(_client, "Scraper.Shows");
                var document = Document.FromJson(doc.ToString());
                await table.PutItemAsync(document);

                lastShowId = showId;
            }

            await Task.Delay(TimeSpan.FromSeconds(15));

            await _sns.PublishAsync("arn:aws:sns:eu-west-1:046957767819:ScraperTasks", JsonConvert.SerializeObject(new
            {
                jobId = jobId,
                pageId = pageIncomplete ? pageId : (pageId + 1),
                lastId = lastShowId ?? lastId
            }));

            return "continue";
        }
    }

    [DynamoDBTable("Scraper.Jobs")]
    public class Job
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public int MaxShowsPerTask { get; set; }
    }
}
