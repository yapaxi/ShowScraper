using Amazon.SimpleNotificationService;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.Bus
{
    public class Bus : IBus
    {
        private readonly string _scraperTaskSNSTopic;
        private readonly IAmazonSimpleNotificationService _sns;

        public Bus(string scraperTaskSNSTopic, IAmazonSimpleNotificationService sns)
        {
            this._scraperTaskSNSTopic = scraperTaskSNSTopic;
            _sns = sns ?? throw new ArgumentNullException(nameof(sns));
        }

        public Task SendScrapPageCommand(string jobId, int pageId, int lastId)
        {
            return _sns.PublishAsync(_scraperTaskSNSTopic, JsonConvert.SerializeObject(new
            {
                jobId = jobId,
                pageId = pageId,
                lastId = lastId
            }));
        }
    }
}
