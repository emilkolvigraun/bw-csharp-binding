using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using BWBinding.Common;
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
        private NetworkStream inputStream;
        private StreamWriter outputStream;

        private Dictionary<int, IResponseHandler> responseHandlers;
        private Dictionary<int, IMessageHandler> messageHandlers;
        private Dictionary<int, IListResultHandler> resultHandlers;
        private Object responseLock;
        private Object messageLock;
        private Object resultLock;


        public BossWaveClient(string server, int portNumber)
        {
            this.server = server;
            this.portNumber = portNumber;

            responseHandlers = new Dictionary<int, IResponseHandler>();
            messageHandlers = new Dictionary<int, IMessageHandler>();
            resultHandlers = new Dictionary<int, IListResultHandler>();
            responseLock = new Object();
            messageLock = new Object();
            resultLock = new Object();
        }

        /**
         * Establishing the connection and creating a ThreadStart delegate */
        public void Connect()
        {
            try
            {
                connection = new TcpClient(server, portNumber);
                inputStream = connection.GetStream();
                outputStream = new StreamWriter(connection.GetStream());
                try
                {
                    Frame frame = Frame.ReadFromStream(inputStream);
                    if (frame.command != Command.HELLO)
                    {
                        Dispose();
                        throw new SystemException("Recieved an invalid BOSSWAVE Acknowledgement. ");
                    }
                }
                catch (CorruptedFrameException ex)
                {
                    Dispose();
                    throw new SystemException(ex.ToString());
                }
                new Thread(new ThreadStart(new BossWaveListener().Run)).Start();
            }
            catch(Exception ex) when (ex is SocketException || ex is IOException)
            {
                throw new Exception("Couldn't connect to the server.\n" + ex.ToString());
            } 
        }

        public void Dispose()
        {
            inputStream.Close();
            outputStream.Close();
            connection.Close();
        }

        /**
         * The following Activation methods are used to install the individual handlers.
         * The LOCK keyword restricts code from being executed by more than one thread at the same time.
         * This makes threaded programs reliable.
         */
        private void ActivateResultHandler(int sequenceNumber, IListResultHandler listResultHandler)
        {
            lock (resultLock)
            {
                resultHandlers.Add(sequenceNumber, listResultHandler);
            }
        }

        private void ActivateMessageHandler(int sequenceNumber, IMessageHandler messageHandler)
        {
            lock (messageLock)
            {
                messageHandlers.Add(sequenceNumber, messageHandler);
            }
        }

        private void ActivateResponseHandler(int sequenceNumber, IResponseHandler responseHandler)
        {
            lock (responseLock)
            {
                responseHandlers.Add(sequenceNumber, responseHandler);
            }
        }

        /**
         * The following methods are public User actions / abilities
         */
        public void SetEntity()
        {
            // Set Entity file and make Entity
        }

        public string MakeEntity()
        {

            Command command = Command.MAKE_ENTITY;
            return CommandUtils.GetCode(command);
        }

        public string Publish()
        {
            Command command = Command.PUBLISH;
            return CommandUtils.GetCode(command);
        }

        public string Subscribe()
        {
            Command command = Command.SUBSCRIBE;
            return CommandUtils.GetCode(command);
        }

        public string List()
        {
            Command command = Command.LIST;
            return CommandUtils.GetCode(command);
        }

        public string Query()
        {
            Command command = Command.QUERY;
            return CommandUtils.GetCode(command);
        }

        public string MakeDoT()
        {
            Command command = Command.MAKE_DOT;
            return CommandUtils.GetCode(command);
        }

        public string MakeChain()
        {
            Command command = Command.MAKE_CHAIN;
            return CommandUtils.GetCode(command);
        }
    }
}
