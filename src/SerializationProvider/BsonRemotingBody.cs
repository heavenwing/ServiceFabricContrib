using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServiceFabricContrib
{
    //NOTE WrappedMessage can be used in Bson
    [Obsolete]
    public class BsonRemotingBody : WrappedMessage, IServiceRemotingRequestMessageBody, IServiceRemotingResponseMessageBody
    {
        public BsonRemotingBody(object wrapped)
        {
            this.Value = wrapped;
        }

        public void SetParameter(int position, string parameName, object parameter)
        {  //Not Needed if you are using WrappedMessage
            throw new NotImplementedException();
        }

        public object GetParameter(int position, string parameName, Type paramType)
        {
            //Not Needed if you are using WrappedMessage
            throw new NotImplementedException();
        }

        public void Set(object response)
        { //Not Needed if you are using WrappedMessage
            throw new NotImplementedException();
        }

        public object Get(Type paramType)
        {  //Not Needed if you are using WrappedMessage
            throw new NotImplementedException();
        }
    }
}
