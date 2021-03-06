﻿using Autofac;
using Shots.Services.NavigationService;
using Shots.Web.Services.DesignTime;
using Shots.Web.Services.Interface;
using Shots.Web.Services.RunTime;

namespace Shots.AppEngine.Modules
{
    internal class ServiceModule : AppModule
    {
        public override void LoadDesignTime(ContainerBuilder builder)
        {
            builder.RegisterType<DesignNavigationService>().As<INavigationService>();
            builder.RegisterType<DesignShotsService>().As<IShotsService>();
        }

        public override void LoadRunTime(ContainerBuilder builder)
        {
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<ShotsService>().As<IShotsService>();
        }
    }
}