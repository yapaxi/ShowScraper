using Lambda.ShowScraper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Lambda
{
    [TestClass]
    public class LambdaUnitTests
    {
        private static readonly string JOB_ID = "job-12345";

        [TestMethod]
        [DataRow(1, 1, 20)]
        [DataRow(1, 10, 20)]
        [DataRow(1, 19, 20)]
        [DataRow(2, 1, 20)]
        [DataRow(2, 10, 20)]
        [DataRow(2, 19, 20)]

        [DataRow(2, 1, 1)]

        [DataRow(2, 10, 1)]
        [DataRow(2, 19, 1)]
        [DataRow(2, 10, 5)]
        [DataRow(2, 19, 5)]
        public async Task HappyPath(int pages, int showsPerPage, int maxShowsPerTask)
        {
            var results = new List<JObject>();
            var commandHistory = new List<string>();
            var functionObject = Setup(pages, showsPerPage, maxShowsPerTask, null, results, commandHistory);
            var function = functionObject.Object;

            var sqsEvent = MakeSQSEvent(JsonConvert.SerializeObject(new
            {
                jobId = JOB_ID,
                pageId = 0,
                lastId = -1
            }));

            var counter = 0;
            while (true)
            {
                ++counter;
                Console.WriteLine(sqsEvent.Records[0].Body);
                Console.WriteLine($"iteration {counter}");

                var result = await function.Handle(sqsEvent);
                if (result != "continue")
                {
                    break;
                }
                sqsEvent = MakeSQSEvent(commandHistory.Last());

                if (counter >= 100)
                {
                    Assert.Fail("To many iterations");
                }
            }

            Assert.AreEqual(showsPerPage * pages, results.Count);
            
            foreach (var show in results)
            {
                var cast = show["cast"].ToArray();
                var order = cast.Select(e => e["orderId"].Value<int>()).ToArray();
                Assert.IsTrue(order.SequenceEqual(order.OrderBy(e => e)));
            }
        }

        private static Amazon.Lambda.SQSEvents.SQSEvent MakeSQSEvent(string body)
        {
            return new Amazon.Lambda.SQSEvents.SQSEvent()
            {
                Records = new List<Amazon.Lambda.SQSEvents.SQSEvent.SQSMessage>()
                {
                    new Amazon.Lambda.SQSEvents.SQSEvent.SQSMessage()
                    {
                        Body = body
                    }
                }
            };
        }

        private static Mock<Function> Setup(
            int pages,
            int showsPerPage, 
            int maxShowsPerTask, 
            int? endPage,
            List<JObject> results,
            List<string> commandHistory)
        {
            var moq = new Mock<Function>();

            moq.Setup(e => e.GetJob(JOB_ID)).ReturnsAsync(new Job()
            {
                Id = JOB_ID,
                EndPage = endPage ?? int.MaxValue,
                MaxShowsPerTask = maxShowsPerTask
            });

            var showIndex = 0;
            for (var pageId = 0; pageId < pages; pageId++)
            {
                var shows = new JArray(Enumerable.Range(0, showsPerPage).Select(e => JObject.FromObject(new
                {
                    id = showIndex++
                })));

                moq.Setup(e => e.GetShows(pageId)).ReturnsAsync(shows);

                foreach (var show in shows)
                {
                    var id = show["id"].Value<int>();
                    moq.Setup(e => e.GetShowWithEmbeddedCast(id)).ReturnsAsync(JObject.FromObject(new
                    {
                        id = id,
                        _embedded = new
                        {
                            cast = new[]
                            {
                                new { person = new Person { birthday = "2010-01-01" }, orderId = 3 },
                                new { person = new Person { birthday = "2010-01-03" }, orderId = 1 },
                                new { person = new Person { birthday = "2009-00-00" }, orderId = 5 },
                                new { person = new Person { birthday = "2009-05-00" }, orderId = 4 },
                                new { person = new Person { birthday = (string)null }, orderId = 6 },
                                new { person = (Person)null, orderId = 6 },
                                new { person = new Person { birthday = "2010-01-02" }, orderId = 2 },
                            }
                        }
                    }));
                }
            }

            moq.Setup(e => e.GetShows(pages)).ReturnsAsync((JArray)null);

            moq
                .Setup(e => e.SaveShow(It.IsAny<JObject>()))
                .Callback((JObject o) =>
                {
                    results.Add(o);
                })
                .Returns(Task.CompletedTask);

            moq.Setup(e => e.SendScrapPageCommand(It.IsAny<string>()))
                .Callback((string o) =>
                {
                    commandHistory.Add(o);
                })
                .Returns(Task.CompletedTask);

            return moq;
        }

        private class Person
        {
            public string birthday { get; set; }
        }
    }
}
