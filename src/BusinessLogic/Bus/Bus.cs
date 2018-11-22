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

        public Task SendProcessTaskCommand(string taskId)
        {
            return _sns.PublishAsync(_scraperTaskSNSTopic, JsonConvert.SerializeObject(new
            {
                taskId = taskId
            }));
        }
    }
}
