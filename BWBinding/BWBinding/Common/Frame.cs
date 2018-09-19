using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using BWBinding.Exceptions;
using BWBinding.Utils;

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

            try
            {
                inputStream.Read(frameBytes, 0, BW_HEADER_LENGTH);
            }
            catch (IOException ex)
            {
                throw new IOException("The Header is corrupted.", ex);
            }

            string[] authorizationTokens = frameHeader.Trim().Split(' ');

            if (authorizationTokens.Length != 3)
            {
                throw new CorruptedFrameException("Frame header must contain 3 fields.");
            }
            
            Command command = CommandUtils.GetCommand(authorizationTokens[0]);

            int frameLength;
            try
            {
                frameLength = int.Parse(authorizationTokens[1]);
            }
            catch (FormatException ex)
            {
                throw new CorruptedFrameException("The length of the Frame Header is invalid: ", ex);
            }

            int sequenceNumber;
            try
            {
                sequenceNumber = int.Parse(authorizationTokens[2]);
            }
            catch (FormatException ex)
            {
                throw new CorruptedFrameException("The sequence number is invalid: ", ex);
            }
            
            List<VSKeyPair> vsKeyPairs;
            List<PayloadObject> payloadObjects;
            List<RoutingObject> routingObjects;
        }
    }
}
