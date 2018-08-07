using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContribSample.Contracts
{
    public interface ISampleRemotingService : IService
    {
        Task<List<PeopleDto>> GetPeoplesAsync(FilterDto filter);
    }
}
