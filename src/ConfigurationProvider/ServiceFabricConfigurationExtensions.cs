using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Text;

namespace ServiceFabricContrib
{
    public static class ServiceFabricConfigurationExtensions
    {
        public static IConfigurationBuilder AddServiceFabricConfiguration(this IConfigurationBuilder builder,
            ServiceContext serviceContext, string configPackageName = "Config")
        {
            builder.Add(new ServiceFabricConfigurationSource(serviceContext, configPackageName));
            return builder;
        }
    }
}
