using Microsoft.Extensions.Configuration;
using System.Fabric;

namespace ServiceFabricContrib
{
    public class ServiceFabricConfigurationProvider : ConfigurationProvider
    {
        private readonly ServiceContext serviceContext;
        private readonly string _configPackageName;

        public ServiceFabricConfigurationProvider(ServiceContext serviceContext, string configPackageName)
        {
            this.serviceContext = serviceContext;
            _configPackageName = configPackageName;
        }

        public override void Load()
        {
            var config = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject(_configPackageName);
            foreach (var section in config.Settings.Sections)
            {
                foreach (var parameter in section.Parameters)
                {
                    Data[$"{section.Name}{ConfigurationPath.KeyDelimiter}{parameter.Name}"] = parameter.Value;
                }
            }
        }
    }
}
