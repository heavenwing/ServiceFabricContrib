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
    public class BsonResponseMessageBodySerializer : IServiceRemotingResponseMessageBodySerializer
    {
        public IOutgoingMessageBody Serialize(IServiceRemotingResponseMessageBody serviceRemotingRequestMessageBody)
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

        public IServiceRemotingResponseMessageBody Deserialize(IIncomingMessageBody messageBody)
        {
            var reader = new BsonDataReader(messageBody.GetReceivedBuffer());
            var serializer = new JsonSerializer();
            return serializer.Deserialize<BsonRemotingResponseBody>(reader);
        }
    }
}
