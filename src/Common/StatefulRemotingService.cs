using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Text;

namespace ServiceFabricContrib
{
    public abstract class StatefulRemotingService
    {
        public StatefulRemotingService(StatefulServiceContext serviceContext,
          IReliableStateManager stateManager)
        {
            ServiceContext = serviceContext;
            StateManager = stateManager;
        }

        protected StatefulServiceContext ServiceContext { get; }

        protected IReliableStateManager StateManager { get; }
    }
}
