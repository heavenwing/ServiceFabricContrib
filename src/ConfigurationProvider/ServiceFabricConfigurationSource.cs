

namespace ServiceFabricContrib
{
#if NETSTANDARD2_0
    using Microsoft.Extensions.Configuration;
    using System.Fabric;

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
#endif
}
