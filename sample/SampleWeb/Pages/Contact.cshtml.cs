using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContribSample.Contracts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ServiceFabric.Services.Remoting.Client;
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

            var proxy = ServiceProxy.Create<ISampleRemotingService>(builder.ToUri());

            Peoples = await proxy.GetPeoplesAsync();
        }
    }
}
