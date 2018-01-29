using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContribSample.Contracts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using ServiceFabricContrib;

namespace SampleWeb.Pages
{
    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        public int Count { get; set; }

        public async Task OnGetAsync()
        {
            var builder = new ServiceUriBuilder("SampleActorService");
            var actorId = new ActorId(1);

            var actorService = ActorServiceProxy.Create<IActorServiceEx>(builder.ToUri(), actorId);

            try
            {
                var exists = await actorService.ActorExistsAsync(actorId);

                var actor = ActorProxy.Create<ISampleActor>(actorId, builder.ToUri());
                Count = await actor.GetCountAsync(default(CancellationToken));
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            Message = "Your application description page.";
        }
    }
}
