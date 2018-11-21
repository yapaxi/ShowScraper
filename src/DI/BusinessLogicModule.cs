using Autofac;
using ShowScraper.BusinessLogic;
using ShowScraper.BusinessLogic.Contracts;
using ShowScraper.BusinessLogic.DataAccess;
using ShowScraper.BusinessLogic.TVMaze;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowScraper.DI
{
    public class BusinessLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StorageProvider>().As<IStorageProvider>().SingleInstance();
            builder.RegisterType<ShowDatabase>().As<IShowDatabase>().SingleInstance();
            builder.Register(e => new ScraperService(
                storageProvider: e.Resolve<IStorageProvider>(),
                showDatabase: e.Resolve<IShowDatabase>(),
                maxScrapers: 4
            )).As<IScraperService>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
