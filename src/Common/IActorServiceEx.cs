using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabricContrib
{
    //ref: https://stackoverflow.com/a/38279820/377727

    public interface IActorServiceEx : IService
    {
        Task<bool> ActorExistsAsync(ActorId actorId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
