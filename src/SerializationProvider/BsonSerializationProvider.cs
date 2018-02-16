using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Newtonsoft.Json;
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

        public static object TryDeserializeObject(object value,Type valueType)
        {
            var str = value?.ToString();
            if (string.IsNullOrEmpty(str)) return null;
            if (valueType == typeof(bool) && (str == "F" || str == "T"))
                return str == "T";
            object result;
            try
            {
                result= JsonConvert.DeserializeObject(str, valueType);
            }
            catch
            {
                try
                {
                    result = Convert.ChangeType(value, valueType);
                }
                catch
                {
                    throw;
                }
            }
            return result;
        }
    }
}
