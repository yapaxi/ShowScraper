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
using Amazon.SQS;
using Amazon.Lambda.SQSEvents;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lambda.ShowScraper
{
    public class Function
    {
        private static readonly AmazonDynamoDBClient _client = new AmazonDynamoDBClient();
        private static readonly DynamoDBContext _context = new DynamoDBContext(_client);
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly IAmazonSQS _sqs = new AmazonSQSClient();

        public async Task<string> Handle(SQSEvent snsEvent)
        {
            var record = snsEvent.Records.Single();

            var message = JObject.Parse(record.Body);
            var jobId = message["jobId"].Value<string>();
            var pageId = message["pageId"].Value<int>();
            var lastId = message["lastId"].Value<int>();


            var job = await GetJob(jobId);

            if (job == null)
            {
                return "job-not-found";
            }

            if (pageId > job.EndPage)
            {
                return "end-page-reached";
            }

            var shows = await GetShows(pageId);

            if (shows == null)
            {
                return "no-more-shows";
            }

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

                var show = await GetShowWithEmbeddedCast(showId);

                var doc = new JObject
                {
                    ["id"] = showId,
                    ["pageId"] = pageId,
                    ["name"] = show["name"]
                };

                var cast = show["_embedded"]["cast"]
                    .OrderByDescending(e => e["person"]?["birthday"]?.Value<DateTime?>() ?? default(DateTime))
                    .ToArray();

                doc["cast"] = new JArray(cast);

                await SaveShow(doc);

                lastShowId = showId;
            }

            var body = JsonConvert.SerializeObject(new
            {
                jobId = jobId,
                pageId = pageIncomplete ? pageId : (pageId + 1),
                lastId = lastShowId ?? lastId
            });

            await _sqs.SendMessageAsync(new Amazon.SQS.Model.SendMessageRequest()
            {
                QueueUrl = "https://sqs.eu-west-1.amazonaws.com/046957767819/ScraperTasks",
                DelaySeconds = 15,
                MessageBody = body,
            });
            
            return "continue";
        }

        protected virtual async Task SaveShow(JObject doc)
        {
            var table = Table.LoadTable(_client, "Scraper.Shows");
            var document = Document.FromJson(doc.ToString());
            await table.PutItemAsync(document);
        }

        protected virtual async Task<JObject> GetShowWithEmbeddedCast(int showId)
        {
            var url = $"http://api.tvmaze.com/shows/{showId}?embed=cast";
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

            response.EnsureSuccessStatusCode();

            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        protected virtual Task<Job> GetJob(string jobId)
        {
            return _context.LoadAsync<Job>(jobId);
        }

        protected virtual async Task<JArray> GetShows(int pageId)
        {
            var pageUrl = $"http://api.tvmaze.com/shows?page={pageId}";
            var pageResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, pageUrl));

            if (pageResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return JArray.Parse(await pageResponse.Content.ReadAsStringAsync());
        }
    }

    [DynamoDBTable("Scraper.Jobs")]
    public class Job
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public int MaxShowsPerTask { get; set; }

        [DynamoDBProperty]
        public int EndPage { get; set; }
    }
}
