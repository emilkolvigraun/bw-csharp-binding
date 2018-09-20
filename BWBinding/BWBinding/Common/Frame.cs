using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using BWBinding.Exceptions;
using BWBinding.Utils;

namespace BWBinding.Common
{
    /**
     * Utilizing the Frame Technology design pattern
     */
    public class Frame
    {
        private static int BW_HEADER_LENGTH = 27;
        public Command command { get; private set; }
        public int sequenceNumber { get; private set; }
        public List<VSKeyPair> vsKeyPairs { get; }
        public List<PayloadObject> payloadObjects { private set; get; }
        public List<RoutingObject> routingObjects { private set; get; }

        public Frame(Command command, int sequenceNumber, List<VSKeyPair> vsKeyPairs, 
            List<PayloadObject> payloadObjects, List<RoutingObject> routingObjects)
        {
            this.command = command;
            this.sequenceNumber = sequenceNumber;
            this.vsKeyPairs = vsKeyPairs;
            this.payloadObjects = payloadObjects;
            this.routingObjects = routingObjects;
        }

        public List<RoutingObject> RoutingObjects
        {
            get { return routingObjects; }
        }

        public byte[] PopFirstValue(string key)
        {
            foreach (VSKeyPair pair in vsKeyPairs)
            {
                if (pair.key.Equals(key))
                {
                    return pair.value;
                }
            }
            return null;
        }

        public static Frame ReadFromStream(Stream inputStream)
        {
            byte[] frameBytes = new byte[BW_HEADER_LENGTH];
            string[] authorizationTokens;
            try
            {
                inputStream.Read(frameBytes, 0, BW_HEADER_LENGTH);         
                string frameHeader = Encoding.UTF8.GetString(frameBytes);
                authorizationTokens = Regex.Split(frameHeader.Trim(), " ");
            }
            catch (IOException ex)
            {
                throw new IOException("The Header is corrupted.\n", ex);
            }

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
                throw new CorruptedFrameException("The length of the Frame Header is invalid.\n", ex);
            }

            int sequenceNumber;
            try
            {
                sequenceNumber = int.Parse(authorizationTokens[2]);
            }
            catch (FormatException ex)
            {
                throw new CorruptedFrameException("The sequence number is invalid.\n", ex);
            }
            
            List<VSKeyPair> vsKeyPairs = new List<VSKeyPair>();
            List<PayloadObject> payloadObjects = new List<PayloadObject>();
            List<RoutingObject> routingObjects = new List<RoutingObject>();
            string currentLine;
            while (!(currentLine = ReadLine(inputStream)).Equals("end"))
            {
                string[] tokens = Regex.Split(currentLine, " ");
                if (tokens.Length != 3)
                {
                    throw new CorruptedFrameException("The Header does not contain three fields but " + tokens.Length + ".");
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
                    throw new CorruptedFrameException("The length of the Header is corrupted: " + currentLine + "\n", ex);
                }

                switch (tokens[0])
                {
                    case "kv":
                        string key = tokens[1];
                        byte[] vkBody = new byte[length];
                        inputStream.Read(vkBody, 0, length);
                        VSKeyPair vsKeyPair = new VSKeyPair(key, vkBody);
                        vsKeyPairs.Add(vsKeyPair);
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
                            throw new CorruptedFrameException("The Payload type is invalid: " + currentLine + "\n", ex);
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
        private static byte[] Read(Stream inputStream, byte end)
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
        private static string ReadLine(Stream inputStream)
        {
            byte[] bytes;
            try
            {
                bytes = Read(inputStream, (byte) '\n');
            }
            catch (IOException ex)
            {
                throw new IOException("Cannot interpret the stream.\n");
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
                   EqualityComparer<List<VSKeyPair>>.Default.Equals(vsKeyPairs, frame.vsKeyPairs) &&
                   EqualityComparer<List<PayloadObject>>.Default.Equals(payloadObjects, frame.payloadObjects) &&
                   EqualityComparer<List<RoutingObject>>.Default.Equals(routingObjects, frame.routingObjects);
        }

        // Mother Writer
        public void Write(Stream outputStream)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(string.Format("{0} 0000000000 {1:0000000000}\n", CommandUtils.GetCode(command), sequenceNumber));
            outputStream.Write(encoded, 0, encoded.Length);
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
            outputStream.Write(Encoding.UTF8.GetBytes("end\n"), 0, Encoding.UTF8.GetBytes("end\n").Length);
        }
    }
}
