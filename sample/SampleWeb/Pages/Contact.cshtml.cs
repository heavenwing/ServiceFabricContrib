using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContribSample.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using ServiceFabricContrib;

namespace SampleWeb.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        public List<PeopleDto> Peoples { get; set; }

        public async Task OnGetAsync()
        {
            Message = "Your contact page.";

            var builder = new ServiceUriBuilder("SampleState");

            //use native serialization
            //var proxy = ServiceProxy.Create<ISampleRemotingService>(builder.ToUri());

            //use bson serialization
            var proxyFactory = new ServiceProxyFactory((c) =>
            {
                return new FabricTransportServiceRemotingClientFactory(
                    serializationProvider: new BsonSerializationProvider());
            });
            var proxy = proxyFactory.CreateServiceProxy<ISampleRemotingService>(builder.ToUri());

            Peoples = await proxy.GetPeoplesAsync(new FilterDto { Search = "abc", Page = 5 });

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
            await bus.Publish(new FilterDto { Search = "abc", Page = 5 });

        }
    }
}
