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
        private JsonSerializer serializer;

        public BsonRequestMessageBodySerializer()
        {
            serializer = new JsonSerializer();
            //JsonSerializer.Create(new JsonSerializerSettings()
            //{
            //    TypeNameHandling = TypeNameHandling.All
            //});
        }

        public IOutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
        {
            if (serviceRemotingRequestMessageBody == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BsonDataWriter(ms))
                {
                    serializer.Serialize(writer, serviceRemotingRequestMessageBody);
                    writer.Flush();

                    var segment = new ArraySegment<byte>(ms.ToArray());
                    var segments = new List<ArraySegment<byte>> { segment };
                    return new OutgoingMessageBody(segments);
                }
            }
        }

        public IServiceRemotingRequestMessageBody Deserialize(IIncomingMessageBody messageBody)
        {
            using (var reader = new BsonDataReader(messageBody.GetReceivedBuffer()))
            {
                return serializer.Deserialize<BsonRemotingRequestBody>(reader);
            }
        }
    }
}
