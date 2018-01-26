using Microsoft.Extensions.Configuration;
using System.Fabric;

namespace ServiceFabricContrib
{
    public class ServiceFabricConfigurationSource : IConfigurationSource
    {
        private readonly ServiceContext serviceContext;
        private readonly string _configPackageName;

        public ServiceFabricConfigurationSource(ServiceContext serviceContext, string configPackageName)
        {
            this.serviceContext = serviceContext;
            _configPackageName = configPackageName;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ServiceFabricConfigurationProvider(serviceContext, _configPackageName);
        }
    }

}
