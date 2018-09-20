using System.Collections.Generic;
using System.Text;
using BWBinding.Common;

namespace BWBinding.Utils
{
    /**
     * Frame Technology 
     */
    class FrameUtils
    {
        private Command command;
        private int sequenceNumber;
        private List<VSKeyPair> vsKeyPairs;
        private List<PayloadObject> payloadObjects;
        private List<RoutingObject> routingObjects;


        public FrameUtils(Command command, int sequenceNumber)
        {
            this.command = command;
            this.sequenceNumber = sequenceNumber;
            vsKeyPairs = new List<VSKeyPair>();
            payloadObjects = new List<PayloadObject>();
            routingObjects = new List<RoutingObject>();
        }

        public void Clear()
        {
            vsKeyPairs.Clear();
            payloadObjects.Clear();
            routingObjects.Clear();
        }

        public FrameUtils SetCommandGetUtils(Command command)
        {
            this.command = command;
            return this;
        }

        public FrameUtils SetSequenceNumberGetUtils(int sequenceNumber)
        {
            this.sequenceNumber = sequenceNumber;
            return this;
        }

        public FrameUtils AddVSKeyPairGetUtils(string key, byte[] value)
        {
            vsKeyPairs.Add(new VSKeyPair(key, value));
            return this;
        }

        public FrameUtils AddVSKeyPairGetUtils(string key, string value)
        {
            vsKeyPairs.Add(new VSKeyPair(key, Encoding.UTF8.GetBytes(value)));
            return this;
        }

        public FrameUtils AddRoutingObjectGetUtils(RoutingObject routingObject)
        {
            routingObjects.Add(routingObject);
            return this;
        }

        public FrameUtils AddPayloadObjectGetUtils(PayloadObject payloadObject)
        {
            payloadObjects.Add(payloadObject);
            return this;
        }

        public Frame Build()
        {
            return new Frame(command, sequenceNumber, vsKeyPairs, payloadObjects, routingObjects);
        }

    }
}
