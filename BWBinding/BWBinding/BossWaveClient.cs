using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BWBinding.Common;
using BWBinding.Control;
using BWBinding.Exceptions;
using BWBinding.Interfaces;
using BWBinding.Observer;
using BWBinding.Utils;

namespace BWBinding
{
    /**
     *  Aggregator class
     *  @Author: Emil Stubbe Kolvig-Raun
     *  Implements IDisposable, c# equivalent to Java's AutoClosable
    **/
    public class BossWaveClient : IDisposable
    {
        private string server;
        private int portNumber;
        private TcpClient connection;
        private Thread listenerThread;

        public BossWaveClient(string server, int portNumber)
        {
            this.server = server;
            this.portNumber = portNumber;

            Controller.Instance.responseHandlers = new Dictionary<int, IResponseHandler>();
            Controller.Instance.messageHandlers = new Dictionary<int, IMessageHandler>();
            Controller.Instance.resultHandlers = new Dictionary<int, IListResultHandler>();
            Controller.Instance.responseLock = new object();
            Controller.Instance.messageLock = new object();
            Controller.Instance.resultLock = new object();
        }

        /**
         * Establishing the connection and creating a Thread delegate */
        public void Connect()
        {
            try
            {
                listenerThread = new Thread(new ThreadStart(new BossWaveListener().Run));
                connection = new TcpClient(server, portNumber);
                Controller.Instance.inputStream = connection.GetStream();
                Controller.Instance.outputStream = new BinaryWriter(connection.GetStream());
                Frame frame = Frame.ReadFromStream(Controller.Instance.inputStream);
                if (frame.command != Command.HELLO)
                {
                    Dispose();
                    throw new SystemException("Recieved an invalid BOSSWAVE Acknowledgement.\n");
                }
                listenerThread.Start();
            }
            catch (Exception ex) when (ex is SocketException || ex is IOException || ex is CorruptedFrameException)
            {
                Dispose();
                if (ex is CorruptedFrameException)
                {
                    throw new SystemException(ex.ToString());
                }
                else
                {
                    throw new Exception("Couldn't connect to the server.\n" + ex.ToString());
                }
            }
        }

        public void Dispose()
        {
            listenerThread.Interrupt();
            Controller.Instance.inputStream.Close();
            Controller.Instance.outputStream.Close();
            connection.Close();
        }

        /**
         * The following Activation methods are used to install the individual handlers.
         * The LOCK keyword restricts code from being executed by more than one thread at the same time.
         * This makes threaded programs reliable.
         */
        private void ActivateResultHandler(int sequenceNumber, IListResultHandler listResultHandler)
        {
            lock (Controller.Instance.resultLock)
            {
                Controller.Instance.resultHandlers.Add(sequenceNumber, listResultHandler);
            }
        }

        private void ActivateMessageHandler(int sequenceNumber, IMessageHandler messageHandler)
        {
            lock (Controller.Instance.messageLock)
            {
                Controller.Instance.messageHandlers.Add(sequenceNumber, messageHandler);
            }
        }

        private void ActivateResponseHandler(int sequenceNumber, IResponseHandler responseHandler)
        {
            lock (Controller.Instance.responseLock)
            {
                Controller.Instance.responseHandlers.Add(sequenceNumber, responseHandler);
            }
        }

        /**
         * The following methods are public User actions / abilities
         */
        public void SetEntity(string filepath, IResponseHandler responseHandler)
        {
            Console.WriteLine(listenerThread.IsAlive);
            try
            {
                using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.ReadByte();
                    fileStream.Read(buffer, 0, (int)fileStream.Length);
                    Console.WriteLine(buffer.ToString());
                    BuildEntity(buffer, responseHandler);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("The path cannot be empty.");
            }
        }

        private void BuildEntity(byte[] fileBytes, IResponseHandler responseHandler)
        {
            int sequenceNumber = Frame.GenerateSequenceNumber();
            FrameUtils frameUtils = new FrameUtils(Command.SET_ENTITY, sequenceNumber);
            PayloadType payloadType = new PayloadType(new byte[] { 1, 0, 1, 2 });
            PayloadObject payloadObject = new PayloadObject(payloadType, fileBytes);
            frameUtils.AddPayloadObjectGetUtils(payloadObject);
            Frame frame = frameUtils.Build();
            frame.Write(Controller.Instance.outputStream);
            Controller.Instance.outputStream.Flush();
            ActivateResponseHandler(sequenceNumber, responseHandler);
        }

        public void Publish(Request request, IResponseHandler responseHandler)
        {
            if (request.type.Equals(RequestType.PUBLISH))
            {
                Command command = Command.PUBLISH;

                if (request.persist)
                {
                    command = Command.PERSIST;
                }

                int sequenceNumber = Frame.GenerateSequenceNumber();
                FrameUtils frameUtils = new FrameUtils(command, sequenceNumber);
                frameUtils.AddVSKeyPairGetUtils("uri", request.uri);

                if (request.persist)
                {
                    frameUtils.SetCommandGetUtils(Command.PERSIST);
                }
                else
                {
                    frameUtils.SetCommandGetUtils(Command.PUBLISH);
                }

                frameUtils.AddVSKeyPairGetUtils("persist", request.persist.ToString());
                if (request.expiry != null)
                {
                    frameUtils.AddVSKeyPairGetUtils("expiry", request.expiry.ToString("yyyy-MM-dd'T'HH:mm:ssXXX"));
                }

                if (request.expiryDelta > 0)
                {
                    frameUtils.AddVSKeyPairGetUtils("expiryDelta", string.Format("%dms", request.expiryDelta));
                }

                if (request.primaryAccessChain != null)
                {
                    frameUtils.AddVSKeyPairGetUtils("primary_access_chain", request.primaryAccessChain);
                }

                frameUtils.AddVSKeyPairGetUtils("doverify", request.ifVerify.ToString());

                if (!request.elaborationLevel.Equals(ChainLevel.UNSPECIFIED))
                {
                    frameUtils.AddVSKeyPairGetUtils("elaborate_pac", request.elaborationLevel.ToString().ToLower());
                }

                if (request.autoChain)
                {
                    frameUtils.AddVSKeyPairGetUtils("autochain", "true");
                }

                foreach (RoutingObject routingObject in request.routingObjects)
                {
                    frameUtils.AddRoutingObjectGetUtils(routingObject);
                }

                foreach (PayloadObject payloadObject in request.payloadObjects)
                {
                    frameUtils.AddPayloadObjectGetUtils(payloadObject);
                }

                Frame publishFrame = frameUtils.Build();
                publishFrame.Write(Controller.Instance.outputStream);
                Controller.Instance.outputStream.Flush();
                ActivateResponseHandler(sequenceNumber, responseHandler);
            }
        }

        public void Subscribe(Request request, IResponseHandler responseHandler, IMessageHandler messageHandler)
        {
            int sequenceNumber = Frame.GenerateSequenceNumber();
            FrameUtils frameUtils = new FrameUtils(Command.SUBSCRIBE, sequenceNumber);
            frameUtils.AddVSKeyPairGetUtils("uri", request.uri);
            if (request.expiry != null)
            {
                frameUtils.AddVSKeyPairGetUtils("expiry", request.expiry.ToString("yyyy-MM-dd'T'HH:mm:ssXXX"));
            }

            if (request.expiryDelta > 0)
            {
                frameUtils.AddVSKeyPairGetUtils("expirydelta", string.Format("%dms", request.expiryDelta));
            }

            if (request.primaryAccessChain != null)
            {
                frameUtils.AddVSKeyPairGetUtils("primary_access_chain", request.primaryAccessChain);
            }

            frameUtils.AddVSKeyPairGetUtils("doverify", request.ifVerify.ToString());
            if (request.elaborationLevel != ChainLevel.UNSPECIFIED)
            {
                frameUtils.AddVSKeyPairGetUtils("elaborate_pac", request.elaborationLevel.ToString().ToLower());
            }

            if (request.autoChain)
            {
                frameUtils.AddVSKeyPairGetUtils("autochain", "true");
            }

            if (!request.leavePacked)
            {
                frameUtils.AddVSKeyPairGetUtils("unpack", "true");
            }

            foreach (RoutingObject routingObject in request.routingObjects)
            {
                frameUtils.AddRoutingObjectGetUtils(routingObject);
            }

            Frame subscribeFrame = frameUtils.Build();
            subscribeFrame.Write(Controller.Instance.outputStream);
            Controller.Instance.outputStream.Flush();

            if (responseHandler != null)
            {
                ActivateResponseHandler(sequenceNumber, responseHandler);
            }

            if (messageHandler != null)
            {
                ActivateMessageHandler(sequenceNumber, messageHandler);
            }
        }

        public string List()
        {
            Command command = Command.LIST;
            return CommandUtils.GetCode(command) + " not yet implemented.";
        }

        public string Query()
        {
            Command command = Command.QUERY;
            return CommandUtils.GetCode(command) + " not yet implemented.";
        }

        public string MakeDoT()
        {
            Command command = Command.MAKE_DOT;
            return CommandUtils.GetCode(command) + " not yet implemented.";
        }

        public string MakeChain()
        {
            Command command = Command.MAKE_CHAIN;
            return CommandUtils.GetCode(command) + " not yet implemented.";
        }
    }
}
