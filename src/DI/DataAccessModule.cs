using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Autofac;
using Amazon.SimpleNotificationService;

namespace ShowScraper.DI
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register<IAmazonSimpleNotificationService>(e => new AmazonSimpleNotificationServiceClient())
                .SingleInstance();

            builder
                .Register<IDynamoDBContext>(e => new DynamoDBContext(new AmazonDynamoDBClient()))
                .SingleInstance();

            builder
                .Register<IAmazonSimpleNotificationService>(e => new AmazonSimpleNotificationServiceClient())
                .SingleInstance();


            base.Load(builder);
        }
    }
}
