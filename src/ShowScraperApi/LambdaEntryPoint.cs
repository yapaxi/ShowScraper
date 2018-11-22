using Amazon.Lambda.AspNetCoreServer;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScraper.Api
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration(e => e.AddJsonFile("appsettings.json")
                                                 .AddEnvironmentVariables()).UseStartup<Startup>();
        }
    }
}
