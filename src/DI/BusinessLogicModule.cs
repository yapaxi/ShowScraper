using Autofac;
using ShowScraper.BusinessLogic;
using ShowScraper.BusinessLogic.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowScraper.DI
{
    public class BusinessLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ScraperService>().As<IScraperService>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
