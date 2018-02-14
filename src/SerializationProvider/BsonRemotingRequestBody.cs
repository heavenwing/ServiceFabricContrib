using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServiceFabricContrib
{
    public class BsonRemotingRequestBody : IServiceRemotingRequestMessageBody
    {
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();

        public void SetParameter(int position, string parameName, object parameter)
        {
            Parameters[parameName] = parameter;
        }

        public object GetParameter(int position, string parameName, Type paramType)
        {
            var value = Parameters[parameName].ToString();
            return JsonConvert.DeserializeObject(value, paramType);
        }
    }
}
