using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(RemotingListener = RemotingListener.V2Listener, RemotingClient = RemotingClient.V2Client)]
namespace ServiceFabricContrib
{
    //ref: https://stackoverflow.com/a/38279820/377727

    public interface IActorServiceEx : IActorService
    {
        Task<bool> ActorExistsAsync(ActorId actorId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
