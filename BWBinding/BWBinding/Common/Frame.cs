using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BWBinding.Common
{
    /**
     * Utilizing the Frame Technology design pattern
     */
    class Frame
    {
        private static readonly int BW_HEADER_LENGTH = 27;
        private Command command { get; }
        private int sequenceNumber { get; }
        private ReadOnlyCollection<VSKeyPair> vsKeyPairs { get; }
        private ReadOnlyCollection<PayloadObject> payloadObjects { get; }
        private ReadOnlyCollection<RoutingObject> routingObjects { get; }

        public Frame(Command command, int sequenceNumber, List<VSKeyPair> vsKeyPairs, 
            List<PayloadObject> payloadObjects, List<RoutingObject> routingObjects)
        {
            this.command = command;
            this.sequenceNumber = sequenceNumber;
            this.vsKeyPairs = vsKeyPairs.AsReadOnly();
            this.payloadObjects = payloadObjects.AsReadOnly();
            this.routingObjects = routingObjects.AsReadOnly();
        }

        public static void ReadFromStream(NetworkStream inputStream)
        {
            byte[] frameBytes = new byte[BW_HEADER_LENGTH];
            string frameHeader = Encoding.UTF8.GetString(frameBytes);
            inputStream.Read(frameBytes, 0, BW_HEADER_LENGTH);

            Command command;
            int sequenceNumber;
            List<VSKeyPair> vsKeyPairs;
            List<PayloadObject> payloadObjects;
            List<RoutingObject> routingObjects;
        }
    }
}
