using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ShowScraper.Api.Middleware;
using ShowScraper.DI;

namespace ShowScraper
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private IContainer _applicationContainer;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
            .AddControllersAsServices()
            .AddJsonOptions(e =>
            {
                e.SerializerSettings.DateFormatString = "yyyy-MM-dd'T'HH:mm:ss.fffZ";
                e.SerializerSettings.Culture = System.Globalization.CultureInfo.InvariantCulture;
                e.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
            
            services.AddLogging(e => e.ClearProviders());

            _applicationContainer = CreateContainer(services);

            return new AutofacServiceProvider(_applicationContainer);
        }

        private IContainer CreateContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new BusinessLogicModule(_configuration));
            builder.RegisterModule(new DataAccessModule());
            builder.Populate(services);
            return builder.Build();
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            NLog.LogManager.LoadConfiguration("nlog.config");
            loggerFactory.AddNLog();

            app.UseMiddleware<LoggingMiddleware>();
            app.UseMvc();

            appLifetime.ApplicationStarted.Register(() =>
            {
                _applicationContainer.Resolve<ILogger<Startup>>().LogInformation("Application has started");
            });

            appLifetime.ApplicationStopped.Register(() =>
            {
                _applicationContainer.Resolve<ILogger<Startup>>().LogInformation("Trying to stop gracefully...");
                _applicationContainer.Dispose();
            });
        }

    }
}