using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
        public Command command { get; private set; }
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
        public static Frame ReadFromStream(NetworkStream inputStream)
        {
            byte[] frameBytes = new byte[BW_HEADER_LENGTH];
            try
            {
                inputStream.Read(frameBytes, 0, BW_HEADER_LENGTH);
            }
            catch (IOException ex)
            {
                throw new IOException("The Header is corrupted.", ex);
            }

            string frameHeader = Encoding.UTF8.GetString(frameBytes);
            string[] authorizationTokens = frameHeader.Split(' ').Select(str => str.Trim()).ToArray();

            if (authorizationTokens.Length != 3)
            {
                throw new CorruptedFrameException("Frame header must have a length of 3.\nCurrent length: " + authorizationTokens.Length);
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
            
            List<VSKeyPair> vsKeyPairs = new List<VSKeyPair>();
            List<PayloadObject> payloadObjects = new List<PayloadObject>();
            List<RoutingObject> routingObjects = new List<RoutingObject>();
            string currentLine;
            while (!(currentLine = ReadLine(inputStream)).Equals("end"))
            {
                string[] tokens = currentLine.Split(' ');
                if (tokens.Length != 3)
                {
                    throw new CorruptedFrameException("The Header does not contain three fields: " + currentLine);
                }

                int length;
                try
                {
                    length = int.Parse(tokens[2]);
                    if (length < 0)
                    {
                        throw new CorruptedFrameException("The length of the Header is negative: " + currentLine);
                    }
                }
                catch (FormatException ex)
                {
                    throw new CorruptedFrameException("The length of the Header is corrupted: " + currentLine, ex);
                }

                switch (tokens[0])
                {
                    case "kv":
                        string key = tokens[1];
                        byte[] vkBody = new byte[length];
                        inputStream.Read(vkBody, 0, length);
                        vsKeyPairs.Add(new VSKeyPair(key, vkBody));
                        inputStream.ReadByte();
                        break;
                    case "ro":
                        int routingNumber;
                        try
                        {
                            routingNumber = int.Parse(tokens[1]);
                            if (routingNumber < 0 || routingNumber > 255)
                            {
                                throw new CorruptedFrameException("The Routing number must be: 0 < routingNumber > 255: " + currentLine);
                            }
                        }
                        catch (FormatException ex)
                        {
                            throw new CorruptedFrameException("The Routing number must be: 0 < routingNumber > 255: " + currentLine);
                        }
                        byte[] routingBody = new byte[length];
                        inputStream.Read(routingBody, 0, length);
                        RoutingObject routingObject = new RoutingObject(routingNumber, routingBody);
                        routingObjects.Add(routingObject);
                        inputStream.ReadByte();
                        break;
                    case "po":
                        PayloadType payloadType;
                        try
                        {
                            payloadType = PayloadUtils.FromString(tokens[1]);
                        }
                        catch (ArgumentException ex)
                        {
                            throw new CorruptedFrameException("The Payload type is invalid: " + currentLine, ex);
                        }
                        byte[] payloadBody = new byte[length];
                        inputStream.Read(payloadBody, 0, length);
                        PayloadObject payloadObject = new PayloadObject(payloadType, payloadBody);
                        payloadObjects.Add(payloadObject);
                        inputStream.ReadByte();
                        break;
                    default:
                        throw new CorruptedFrameException("The Header is invalid: " + currentLine);
                }
            }

            return new Frame(command, sequenceNumber, vsKeyPairs, payloadObjects, routingObjects);
        }
        private static byte[] Read(NetworkStream inputStream, byte end)
        {
            List<byte> bytes = new List<byte>();
            int singleByte = inputStream.ReadByte();
            while (singleByte != -1 && (byte) singleByte != end)
            {
                bytes.Add((byte)singleByte);
                singleByte = inputStream.ReadByte();
            }
            if (singleByte != -1)
            {
                bytes.Add((byte)singleByte); // Manually adding end byte
            }
            byte[] returnedBytes = new byte[bytes.Count];
            for (int i = 0; i < bytes.Count; i++)
            {
                returnedBytes[i] = bytes[i];
            }

            return returnedBytes;
        }
        private static string ReadLine(NetworkStream inputStream)
        {
            byte[] bytes;
            try
            {
                bytes = Read(inputStream, (byte) '\n');
            }
            catch (IOException ex)
            {
                throw new IOException("Cannot interpret the stream. \n");
            }
            string currentLine = Encoding.UTF8.GetString(bytes);
            if (currentLine.EndsWith("\n"))
            {
                return currentLine.Substring(0, currentLine.Length - 1);
            }
            else
            {
                return currentLine;
            }
        }
        public static int GenerateSequenceNumber()
        {
            return Math.Abs(new Random().Next());
        }

        public override bool Equals(object obj)
        {
            var frame = obj as Frame;
            return frame != null &&
                   command == frame.command &&
                   sequenceNumber == frame.sequenceNumber &&
                   EqualityComparer<ReadOnlyCollection<VSKeyPair>>.Default.Equals(vsKeyPairs, frame.vsKeyPairs) &&
                   EqualityComparer<ReadOnlyCollection<PayloadObject>>.Default.Equals(payloadObjects, frame.payloadObjects) &&
                   EqualityComparer<ReadOnlyCollection<RoutingObject>>.Default.Equals(routingObjects, frame.routingObjects);
        }

        // Mother Writer
        public void Write(StreamWriter outputStream)
        {
            outputStream.Write(Encoding.UTF8.GetBytes(string.Format("%s 0000000000 %010d\n", CommandUtils.GetCode(command), sequenceNumber)));
            foreach (VSKeyPair pair in vsKeyPairs)
            {
                pair.Write(outputStream);
            }
            foreach (PayloadObject payload in payloadObjects)
            {
                payload.Write(outputStream);
            }
            foreach (RoutingObject route in routingObjects)
            {
                route.Write(outputStream);
            }
        }
    }
}
