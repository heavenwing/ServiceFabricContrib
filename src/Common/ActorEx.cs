using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricContrib
{
    public class ActorEx : Actor
    {
        protected ActorEx(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            return StateManager.TryAddStateAsync(ActorServiceEx.ExistsMarker, true);
        }

    }
}
