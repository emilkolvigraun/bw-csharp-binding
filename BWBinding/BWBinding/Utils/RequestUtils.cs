
using System;
using System.Collections.Generic;
using System.Security.Policy;
using BWBinding.Common;

namespace BWBinding.Utils
{
    class RequestUtils
    {
        public string uri;
        public bool persist;
        public DateTime expiry;
        public long expiryDelta;
        public string primaryAccessChain;
        public bool ifVerify;
        public ChainLevel elaborationLevel;
        public bool autoChain;
        public List<RoutingObject> routingObjects;
        public List<PayloadObject> payloadObjects;
        public RequestType type;

        public RequestUtils(string uri, RequestType type)
        {
            this.uri = uri;
            this.type = type;
        }

        public RequestUtils SetUri(string uri)
        {
            this.uri = uri;
            return this;
        }

        public RequestUtils SetPersist(bool persist)
        {
            this.persist = persist;
            return this;
        }

        public RequestUtils SetExpiry(DateTime expiry)
        {
            this.expiry = expiry;
            return this;
        }

        public RequestUtils SetExpiryDelta(long expiryDelta)
        {
            this.expiryDelta = expiryDelta;
            return this;
        }

        public RequestUtils SetIfVerify(bool ifVerify)
        {
            this.ifVerify = ifVerify;
            return this;
        }

        public RequestUtils SetPrimaryAccessChain(string primaryAccessChain)
        {
            this.primaryAccessChain = primaryAccessChain;
            return this;
        }

        public RequestUtils SetChainLevel(ChainLevel elaborationLevel)
        {
            this.elaborationLevel = elaborationLevel;
            return this;
        }

        public RequestUtils SetAutoChain(bool autoChain)
        {
            this.autoChain = autoChain;
            return this;
        }

        public RequestUtils AddRoutingObject(RoutingObject routingObject)
        {
            routingObjects.Add(routingObject);
            return this;
        }

        public RequestUtils AddPayloadObject(PayloadObject payloadObject)
        {
            payloadObjects.Add(payloadObject);
            return this;
        }

        public void ClearPayloadObjects()
        {
            payloadObjects.Clear();
        }

        public void ClearRoutingObjects()
        {
            routingObjects.Clear();
        }

        public void ClearAll()
        {
            ifVerify = false;
            elaborationLevel = ChainLevel.UNSPECIFIED;
            autoChain = false;
            routingObjects.Clear();
            payloadObjects.Clear();
        }

        public Request Build(RequestType type)
        {
            if (type.Equals(RequestType.PUBLISH))
            {
                return new Request(RequestType.PUBLISH, uri, persist, expiry, expiryDelta, primaryAccessChain, ifVerify, elaborationLevel, autoChain, routingObjects, payloadObjects);
            }
            else
            {
                return null;
            }
        }
    }
}
