using Owin;

namespace ServiceFabricContrib
{
    public interface IOwinAppBuilder
    {
        void Configuration(IAppBuilder appBuilder);
    }
}