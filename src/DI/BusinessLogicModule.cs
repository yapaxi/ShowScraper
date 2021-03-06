﻿using Autofac;
using Microsoft.Extensions.Configuration;
using ShowScraper.BusinessLogic;
using ShowScraper.BusinessLogic.Bus;
using ShowScraper.BusinessLogic.Contracts;
using ShowScraper.BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowScraper.DI
{
    public class BusinessLogicModule : Module
    {
        private readonly IConfiguration _configuration;

        public BusinessLogicModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<StorageProvider>()
                .As<IStorageProvider>()
                .SingleInstance();
            
            builder.Register(e => new ScraperService(
                storageProvider: e.Resolve<IStorageProvider>(),
                bus: e.Resolve<IBus>(),
                maxScrapers: 4
            )).As<IScraperService>().InstancePerLifetimeScope();

            builder
                .RegisterType<ShowService>()
                .As<IShowService>()
                .InstancePerLifetimeScope();

            builder.Register(e => new Bus(
                scraperTasksSQSQueue: _configuration["sqs:scraper-tasks"],
                sqs: e.Resolve<Amazon.SQS.IAmazonSQS>()
            )).As<IBus>().SingleInstance();

            base.Load(builder);
        }
    }
}
