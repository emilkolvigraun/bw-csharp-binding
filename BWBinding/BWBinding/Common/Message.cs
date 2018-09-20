using System.Collections.Generic;

namespace BWBinding.Common
{
    public class Message
    {
        public string from { get; set; }
        public string uri { get; set; }
        public List<PayloadObject> payloadObjects { get; set; }
        public List<RoutingObject> routingObjects { get; set; }

        public Message(string from, string uri, List<PayloadObject> payloadObjects, List<RoutingObject> routingObjects)
        {
            this.from = from;
            this.uri = uri;
            this.payloadObjects = payloadObjects;
            this.routingObjects = routingObjects;
        }
    }
}
