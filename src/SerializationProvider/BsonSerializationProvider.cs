using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServiceFabricContrib
{
    public class BsonSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(Type serviceInterfaceType,
            IEnumerable<Type> requestBodyTypes)
        {
            return new BsonRequestMessageBodySerializer();
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(Type serviceInterfaceType,
            IEnumerable<Type> responseBodyTypes)
        {
            return new BsonResponseMessageBodySerializer();
        }

        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new BsonRemotingMessageBodyFactory();
        }
    }
}
