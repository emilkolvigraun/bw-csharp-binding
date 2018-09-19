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
                connection = new TcpClient(server, portNumber);
                inputStream = connection.GetStream();
                outputStream = new StreamWriter(connection.GetStream());
                Frame frame = Frame.ReadFromStream(inputStream);

                if (frame.command != Command.HELLO)
                {
                    Dispose();
                    throw new SystemException("Recieved an invalid BOSSWAVE Acknowledgement. ");
                }

                new Thread(() => Listen(inputStream)).Start();
            }
            catch(Exception ex) when (ex is SocketException || ex is IOException || ex is CorruptedFrameException)
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

        private void Listen(NetworkStream inputStream)
        {
            new BossWaveListener().Run(inputStream);
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
