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

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lambda.ShowScraper
{
    public class Function
    {
        public async Task<string> Handle(SNSEvent snsEvent)
        {
            AmazonDynamoDBClient _client = new AmazonDynamoDBClient();
            DynamoDBContext _context = new DynamoDBContext(_client);
            HttpClient _httpClient = new HttpClient();


            var record = snsEvent.Records.Single();

            var tasks = JsonConvert.DeserializeObject<string[]>(record.Sns.Message);

            foreach (var taskId in tasks)
            {
                var task = await _context.LoadAsync<JobTask>(taskId);

                foreach (var showId in task.Shows)
                {
                    var url = $"http://api.tvmaze.com/shows/{WebUtility.UrlDecode(showId)}?embed=cast";
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
            }

            return JsonConvert.SerializeObject(record.Sns.Message);
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
