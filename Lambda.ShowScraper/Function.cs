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

            var taskId = JObject.Parse(record.Sns.Message)["taskId"].Value<string>();

            var task = await _context.LoadAsync<JobTask>(taskId);

            if (task == null)
            {
                return "end";
            }

            foreach (var showId in task.Shows)
            {
                var url = $"http://api.tvmaze.com/shows/{WebUtility.UrlEncode(showId)}?embed=cast";
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
                var show = JObject.Parse(await response.Content.ReadAsStringAsync());
                var doc = new JObject();
                doc["id"] = showId;
                doc["name"] = show["name"];
                doc["cast"] = show["_embedded"]["cast"];
                var table = Table.LoadTable(_client, "Scraper.Shows");
                var document = Document.FromJson(doc.ToString());
                await table.PutItemAsync(document);
            }

            var nextId = int.Parse(taskId.Split(':').Last()) + 1;

            var nextTaskId = string.Join(":", taskId.Split(':').Take(2).Concat(new[] { nextId.ToString() }));

            await Task.Delay(TimeSpan.FromSeconds(20));

            await _sns.PublishAsync("arn:aws:sns:eu-west-1:046957767819:ScraperTasks", JsonConvert.SerializeObject(new
            {
                taskId = nextTaskId
            }));

            return "continue";
        }
    }

    [DynamoDBTable("Scraper.Tasks")]
    public class JobTask
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        
        [DynamoDBProperty]
        public List<string> Shows { get; set; }

        [DynamoDBProperty]
        public bool IsComplete { get; set; }
    }
}
