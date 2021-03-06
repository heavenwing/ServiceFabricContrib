﻿using Microsoft.ServiceFabric.Services.Remoting.V2;
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
        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new BsonRemotingBodyFactory();
        }

        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(Type serviceInterfaceType,
            IEnumerable<Type> requestWrappedTypes, IEnumerable<Type> requestBodyTypes = null)
        {
            return new BsonRequestMessageBodySerializer();
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(Type serviceInterfaceType,
            IEnumerable<Type> responseWrappedTypes, IEnumerable<Type> responseBodyTypes = null)
        {
            return new BsonResponseMessageBodySerializer();
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
