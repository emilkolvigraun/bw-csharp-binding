using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BWBinding.Common;
using BWBinding.Interfaces;
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
        private Int32 portNumber;
        private TcpClient connection;
        private NetworkStream inputStream;

        private Dictionary<int, IResponseHandler> responseHandlers;
        private Dictionary<int, IMessageHandler> messageHandlers;
        private Dictionary<int, IListResultHandler> resultHandlers;
        private readonly Object responseLock;
        private readonly Object messageLock;
        private readonly Object resultLock;


        public BossWaveClient(string server, Int32 portNumber)
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
                new Thread(new ThreadStart(new BossListener().Run)).Start();
            }
            catch(IOException e)
            {
                Console.WriteLine(e);
            } 
        }

        public void Dispose()
        {
            inputStream.Close();
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
         * The following methods are public User abilities
         */
        public void SetEntity()
        {
            // Set Entity file and make Entity
        }

        public string MakeEntity()
        {

            Command command = Command.MAKE_ENTITY;
            return CommandUtils.StringValueOf(command);
        }

        public string Publish()
        {
            Command command = Command.PUBLISH;
            return CommandUtils.StringValueOf(command);
        }

        public string Subscribe()
        {
            Command command = Command.SUBSCRIBE;
            return CommandUtils.StringValueOf(command);
        }

        public string List()
        {
            Command command = Command.LIST;
            return CommandUtils.StringValueOf(command);
        }

        public string Query()
        {
            Command command = Command.QUERY;
            return CommandUtils.StringValueOf(command);
        }

        public string MakeDoT()
        {
            Command command = Command.MAKE_DOT;
            return CommandUtils.StringValueOf(command);
        }

        public string MakeChain()
        {
            Command command = Command.MAKE_CHAIN;
            return CommandUtils.StringValueOf(command);
        }
    }
}
