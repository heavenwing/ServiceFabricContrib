using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricContrib
{
    //ref: https://dzimchuk.net/implementing-a-rest-client-for-internal-communication-in-service-fabric/

    /// <summary>
    /// A CommunicationClient for rest api
    /// </summary>
    public class RestApiCommunicationClient<T> : ICommunicationClient
    {
        private readonly Func<Task<T>> apiFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiFactory"></param>
        public RestApiCommunicationClient(Func<Task<T>> apiFactory)
        {
            this.apiFactory = apiFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<T> CreateApiClient() => apiFactory();

        ResolvedServiceEndpoint ICommunicationClient.Endpoint { get; set; }
        string ICommunicationClient.ListenerName { get; set; }
        ResolvedServicePartition ICommunicationClient.ResolvedServicePartition { get; set; }
    }
}
