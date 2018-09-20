using System.Collections.Generic;

namespace BWBinding.Common
{
    public class Message
    {
        private string from { get; }
        private string uri { get; }
        private List<PayloadObject> payloadObjects { get; }
        private List<RoutingObject> routingObjects { get; }

        public Message(string from, string uri, List<PayloadObject> payloadObjects, List<RoutingObject> routingObjects)
        {
            this.from = from;
            this.uri = uri;
            this.payloadObjects = payloadObjects;
            this.routingObjects = routingObjects;

        }
    }
}
