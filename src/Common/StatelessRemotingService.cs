using System;
using System.Collections.Generic;
using System.Fabric;
using System.Text;

namespace ServiceFabricContrib
{
    public abstract class StatelessRemotingService
    {
        public StatelessRemotingService(StatelessServiceContext serviceContext)
        {
            ServiceContext = serviceContext;
        }

        protected StatelessServiceContext ServiceContext { get; }
    }
}
