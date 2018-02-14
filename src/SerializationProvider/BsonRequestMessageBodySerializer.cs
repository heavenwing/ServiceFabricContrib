using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServiceFabricContrib
{
    public class BsonRequestMessageBodySerializer : IServiceRemotingRequestMessageBodySerializer
    {
        public OutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
        {
            if (serviceRemotingRequestMessageBody == null) return null;

            using (var ms = new MemoryStream())
            {
                var writer = new BsonDataWriter(ms);
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, serviceRemotingRequestMessageBody);

                var segment = new ArraySegment<byte>(ms.ToArray());
                var segments = new List<ArraySegment<byte>> { segment };
                return new OutgoingMessageBody(segments);
            }
        }

        public IServiceRemotingRequestMessageBody Deserialize(IncomingMessageBody messageBody)
        {
            var reader = new BsonDataReader(messageBody.GetReceivedBuffer());
            var serializer = new JsonSerializer();
            return serializer.Deserialize<BsonRemotingRequestBody>(reader);
        }
    }
}
