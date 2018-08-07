using Microsoft.ServiceFabric.Services.Remoting.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceFabricContrib
{
    public class BsonRemotingBodyFactory : IServiceRemotingMessageBodyFactory
    {
        public IServiceRemotingRequestMessageBody CreateRequest(string interfaceName, string methodName,
            int numberOfParameters, object wrappedRequestObject)
        {
            return new BsonRemotingRequestBody();
        }

        public IServiceRemotingResponseMessageBody CreateResponse(string interfaceName, string methodName,
            object wrappedResponseObject)
        {
            return new BsonRemotingResponseBody();

        }
    }
}
