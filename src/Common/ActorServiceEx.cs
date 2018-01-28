using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabricContrib
{
    public class ActorServiceEx : ActorService, IActorServiceEx
    {
        public ActorServiceEx(StatefulServiceContext context, ActorTypeInformation actorTypeInfo, Func<ActorService, ActorId, ActorBase> actorFactory = null, Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null, IActorStateProvider stateProvider = null, ActorServiceSettings settings = null)
       : base(context, actorTypeInfo, actorFactory, stateManagerFactory, stateProvider, settings)
        {
        }

       
        public async Task<bool> ActorExistsAsync(ActorId actorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            const int batchSize = 1000;
            ContinuationToken token = null;
            do
            {
                var actors = await StateProvider.GetActorsAsync(batchSize, token, cancellationToken);
                if (actors.Items.Contains(actorId))
                {
                    return true;
                }
                token = actors.ContinuationToken;
            } while (token != null);
            return false;
        }
    }
}
