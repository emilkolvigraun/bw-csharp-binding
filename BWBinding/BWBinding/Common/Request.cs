using System;
using System.Collections.Generic;

namespace BWBinding.Common
{
    public class Request
    {
        public string uri { get; set; }
        public bool persist { get; set; }
        public DateTime expiry { get; set; }
        public long expiryDelta { get; set; }
        public bool ifVerify { get; set; }
        public string primaryAccessChain { get; set; }
        public ChainLevel elaborationLevel { get; set; }
        public bool autoChain { get; set; }
        public List<RoutingObject> routingObjects { get; set; }
        public List<PayloadObject> payloadObjects { get; set; }
        public RequestType type { get; set; }

        public Request(RequestType type, string uri, bool persist, DateTime expiry, long expiryDelta, string primaryAccessChain,
            bool ifVerify, ChainLevel elaborationLevel, bool autoChain, List<RoutingObject> routingObjects,
            List<PayloadObject> payloadObjects)
        {
            this.uri = uri;
            this.type = type;
            this.persist = persist;
            this.expiry = expiry;
            this.expiryDelta = expiryDelta;
            this.primaryAccessChain = primaryAccessChain;
            this.ifVerify = ifVerify;
            this.elaborationLevel = elaborationLevel;
            this.autoChain = autoChain;
            this.routingObjects = routingObjects;
            this.payloadObjects = payloadObjects;
        }
    }
}
