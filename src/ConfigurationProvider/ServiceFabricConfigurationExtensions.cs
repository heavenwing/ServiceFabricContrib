using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Reflection;
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

        /// <summary>
        /// 添加JsonFile、SF、UserSecrets、EnvironmentVariables配置
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseCommonConfiguration(this IWebHostBuilder builder, ServiceContext serviceContext)
        {
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                config.AddServiceFabricConfiguration(serviceContext);

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                config.AddEnvironmentVariables();
            });

            return builder;
        }

        /// <summary>
        /// 添加JsonFile、UserSecrets、EnvironmentVariables配置
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseCommonConfiguration(this IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                config.AddEnvironmentVariables();
            });

            return builder;
        }
    }
}
