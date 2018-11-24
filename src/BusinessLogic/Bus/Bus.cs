using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.Bus
{
    public class Bus : IBus
    {
        private readonly string _scraperTasksSQSQueue;
        private readonly IAmazonSQS _sqs;

        public Bus(string scraperTasksSQSQueue, IAmazonSQS sqs)
        {
            _scraperTasksSQSQueue = scraperTasksSQSQueue;
            _sqs = sqs ?? throw new ArgumentNullException(nameof(sqs));
        }

        public Task SendScrapPageCommand(string jobId, int pageId, int lastId)
        {
            var body = JsonConvert.SerializeObject(new
            {
                jobId = jobId,
                pageId = pageId,
                lastId = lastId
            });

            return _sqs.SendMessageAsync(new SendMessageRequest()
            {
                MessageBody = body,
                DelaySeconds = 15,
                QueueUrl = _scraperTasksSQSQueue
            });
        }
    }
}
