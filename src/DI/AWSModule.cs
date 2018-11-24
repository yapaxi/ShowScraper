using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Autofac;
using Amazon.SQS;

namespace ShowScraper.DI
{
    public class AWSModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<AmazonDynamoDBClient>()
                .SingleInstance();

            builder
                .Register<IDynamoDBContext>(e => new DynamoDBContext(e.Resolve<AmazonDynamoDBClient>()))
                .SingleInstance();

            builder
                .Register<IAmazonSQS>(e => new AmazonSQSClient())
                .SingleInstance();


            base.Load(builder);
        }
    }
}
